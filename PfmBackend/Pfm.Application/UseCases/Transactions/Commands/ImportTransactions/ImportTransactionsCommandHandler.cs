using AutoMapper;
using CsvHelper;
using MediatR;
using Microsoft.Extensions.Logging;
using Pfm.Application.Common;
using Pfm.Application.Exceptions;
using Pfm.Application.Interfaces;
using Pfm.Application.UseCases.Transactions.Mappings;
using Pfm.Domain.Entities;
using Pfm.Domain.Exceptions;
using Pfm.Domain.Interfaces;
using System.Globalization;


namespace Pfm.Application.UseCases.Transactions.Commands.ImportTransactions
{
    public class ImportTransactionsCommandHandler : IRequestHandler<ImportTransactionsCommand, Unit>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<ImportTransactionsCommandHandler> _logger;
        private readonly ITransactionCsvParser _csvParser;

        public ImportTransactionsCommandHandler(
            IUnitOfWork uow,
            IMapper mapper,
            ILogger<ImportTransactionsCommandHandler> logger,
            ITransactionCsvParser csvParser)
        {
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
            _csvParser = csvParser;
        }

        public async Task<Unit> Handle(ImportTransactionsCommand request, CancellationToken cancellationToken)
        {
            // 1. Parse CSV with basic validation
            var parseResult = await _csvParser.ParseAsync(request.CsvStream);

            // 2. Apply DTO validation
            var validator = new ImportTransactionsDtoValidator();
            foreach (var record in parseResult.ValidRecords.ToList())
            {
                var validationResult = await validator.ValidateAsync(record);
                if (!validationResult.IsValid)
                {
                    parseResult.ValidRecords.Remove(record);
                    parseResult.Errors.AddRange(validationResult.Errors.Select(e =>
                        new RecordError(record.Id, e.ErrorCode, e.ErrorMessage)));
                }
            }

            // Short-circuit if any CSV or DTO errors exist
            if (parseResult.HasErrors)
            {
                throw new AppException(
                    parseResult.Errors.Select(e => new AppError(e.RecordId, e.ErrorCode, e.Message)).ToList(),
                    "CSV/DTO validation failed");
            }

            // 3. Apply Domain (business) validation
            var domainErrors = new List<AppError>();
            var validTransactions = new List<Transaction>();

            foreach (var record in parseResult.ValidRecords)
            {
                try
                {
                    var transaction = _mapper.Map<Transaction>(record);
                    transaction.Validate(); 
                    validTransactions.Add(transaction);
                }
                catch (DomainException ex)
                {
                    domainErrors.Add(new AppError(
                        record.Id,
                        ex.ErrorCode,
                        ex.Message));
                }
            }

            if (domainErrors.Any())
            {
                throw new DomainValidationException(domainErrors);
            }

            await _uow.Transactions.AddRangeAsync(validTransactions);
            await _uow.CompleteAsync();

            return Unit.Value;
        }
    }  
}
