using AutoMapper;
using FluentValidation;
using MediatR;
using Pfm.Application.Common;
using Pfm.Application.Interfaces;
using Pfm.Domain.Entities;
using Pfm.Domain.Exceptions;
using System.Text;


namespace Pfm.Application.UseCases.Transactions.Commands.ImportTransactions
{
    public class ImportTransactionsCommandHandler : IRequestHandler<ImportTransactionsCommand, Unit>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ITransactionsCsvParser _csvParser;
        private readonly IValidator<ImportTransactionsDto> _dtoValidator;
        private readonly IValidator<ImportTransactionsCommand> _commandValidator;

        public ImportTransactionsCommandHandler(
            IUnitOfWork uow,
            IMapper mapper,
            ITransactionsCsvParser csvParser,
            IValidator<ImportTransactionsDto> dtoValidator,
            IValidator<ImportTransactionsCommand> commandValidator)
        {
            _uow = uow;
            _mapper = mapper;
            _csvParser = csvParser;
            _dtoValidator = dtoValidator;
            _commandValidator = commandValidator;
        }

        public async Task<Unit> Handle(ImportTransactionsCommand request, CancellationToken ct)
        {

            // Validate the command
            var commandValidation = await _commandValidator.ValidateAsync(request, ct);
            if (!commandValidation.IsValid)
            {
                throw new ValidationProblemException(
                    commandValidation.Errors.Select(e =>
                        new ValidationError("csv", e.ErrorCode, e.ErrorMessage))
                );
            }

            // Parse CSV with basic validation
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(request.CsvContent));
            var parseResult = await _csvParser.ParseAsync(stream);

            // Short-circuit if CSV parsing failed
            if (parseResult.HasErrors)
            {
                parseResult.ThrowIfErrors();
            }

            // Process each record with full validation
            var validationErrors = new List<ValidationError>();
            var validTransactions = new List<Transaction>();

            // HashSet for checking if a Transaction has already appeared (by id)
            var seenTransactionIds = new HashSet<string>();

            foreach (var record in parseResult.ValidRecords)
            {
                // DTO validation
                var validationResult = await _dtoValidator.ValidateAsync(record, ct);
                if (!validationResult.IsValid)
                {
                    validationErrors.AddRange(validationResult.Errors.Select(e =>
                        new ValidationError(record.Id, e.ErrorCode, e.ErrorMessage)));
                    continue;
                }

                // Business validation
                try
                {
                    if (!seenTransactionIds.Add(record.Id))
                    {
                        throw new BusinessRuleException("duplicate-transaction", $"Duplicate transaction ID found: {record.Id}");
                    }

                    var transaction = _mapper.Map<Transaction>(record);

                    // Needed for PosgreSQL compatibility
                    if (transaction.Date.Kind == DateTimeKind.Unspecified)
                    {
                        transaction.Date = DateTime.SpecifyKind(transaction.Date, DateTimeKind.Utc);
                    }

                    transaction.Validate();
                    validTransactions.Add(transaction);
                }

                // Here we just rethrow because we don't want the BusinessRuleExceptions to be masked as ValidationErrors
                catch (BusinessRuleException ex)
                {
                    throw ex;
                } 
                
                catch(Exception ex)
                {
                    validationErrors.Add(new ValidationError(record.Id, "mapping-failed", ex.Message));
                }
                
            }

            if (validationErrors.Any())
            {
                throw new ValidationProblemException(validationErrors);
            }

            await _uow.Transactions.AddRangeAsync(validTransactions);
            await _uow.CompleteAsync();

            return Unit.Value;
        }
    }
}  

