using AutoMapper;
using CsvHelper;
using FluentValidation;
using MediatR;
using Pfm.Application.Common;
using Pfm.Application.Interfaces;
using Pfm.Domain.Entities;
using Pfm.Domain.Exceptions;
using Pfm.Domain.Interfaces;
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

            // 0. Validate the command
            var commandValidation = await _commandValidator.ValidateAsync(request, ct);
            if (!commandValidation.IsValid)
            {
                throw new ValidationProblemException(
                    commandValidation.Errors.Select(e =>
                        new ValidationError("file", e.ErrorCode, e.ErrorMessage))
                );
            }

            // 1. Parse CSV with basic validation
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(request.CsvContent));
            var parseResult = await _csvParser.ParseAsync(stream);

            // Short-circuit if CSV parsing failed
            if (parseResult.HasErrors)
            {
                parseResult.ThrowIfErrors();
            }

            // 3. Process each record with full validation
            var validationErrors = new List<ValidationError>();
            var validTransactions = new List<Transaction>();

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

                try
                {
                    var transaction = _mapper.Map<Transaction>(record);
                    transaction.Validate();
                    validTransactions.Add(transaction);
                }

                // Here we just rethrow because we don't want the BusinessRuleExceptions to be masked as ValidationErrors
                catch (BusinessRuleException ex)
                {
                    throw ex;
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

