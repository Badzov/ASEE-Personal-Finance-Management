using AutoMapper;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Pfm.Application.Common;
using Pfm.Application.DTOs.Requests;
using Pfm.Application.Exceptions;
using Pfm.Application.Mappings.Transactions;
using Pfm.Domain.Entities;
using Pfm.Domain.Interfaces;
using System.Globalization;
using static Pfm.Domain.Interfaces.ITransactionService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Pfm.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(IUnitOfWork uow, IMapper mapper, ILogger<TransactionService> logger)
        {
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<Transaction>> GetTransactionsAsync()
        {
            return (await _uow.Transactions.GetAllAsync()).ToList();
        }

        public async Task ImportTransactionsAsync(Stream csvStream)
        {
            var errors = new List<AppError>();
            var transactions = new List<Transaction>();

            await MapTransactionsCsv(csvStream, errors, transactions);

            if (errors.Any())
            {
                throw new AppException(errors, "Mapping failed");
            }

            if (!transactions.Any())
            {
                return;
            }

            //Implement validation!!!

            await _uow.Transactions.AddRangeAsync(transactions);
            await _uow.CompleteAsync();
        }

        private async Task MapTransactionsCsv(Stream csvStream, List<AppError> errors, List<Transaction> transactions)
        {
            StreamReader reader = new StreamReader(csvStream);
            CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<TransactionCsvMap>();

            try
            {
                await foreach (var record in csv.GetRecordsAsync<TransactionCsvRequest>())
                {
                    try
                    {
                        var transaction = _mapper.Map<Transaction>(record);
                        transactions.Add(transaction);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new AppError(
                            Tag: "record-" + record.Id,
                            Error: "invalid-csv-format",
                            Message: ex.Message
                        ));
                    }
                }
            }
            catch (CsvHelperException ex)
            {
                errors.Add(new AppError(
                    Tag: "csv-exception",
                    Error: "invalid-csv",
                    Message: ex.Message
                ));
            }
        }
    }
}
