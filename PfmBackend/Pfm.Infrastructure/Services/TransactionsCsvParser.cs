using CsvHelper;
using Pfm.Application.Common;
using Pfm.Application.Interfaces;
using Pfm.Application.UseCases.Transactions.Commands.ImportTransactions;
using Pfm.Application.UseCases.Transactions.Mappings;
using System.Globalization;

namespace Pfm.Infrastructure.Services
{
    public class TransactionCsvParser : ITransactionCsvParser
    {
        public async Task<ImportResult<ImportTransactionsDto>> ParseAsync(Stream csvStream)
        {
            var result = new ImportResult<ImportTransactionsDto>();

            try
            {
                using var reader = new StreamReader(csvStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Context.RegisterClassMap<TransactionCsvMap>();

                await foreach (var record in csv.GetRecordsAsync<ImportTransactionsDto>())
                {
                    var recordId = record.Id ?? "unknown";

                    if (string.IsNullOrWhiteSpace(record.Id))
                    {
                        result.Errors.Add(new(recordId, "missing-id", "Transaction ID is required"));
                        continue;
                    }

                    result.ValidRecords.Add(record);
                }
            }
            catch (CsvHelperException ex)
            {
                result.Errors.Add(new("file", "invalid-csv", ex.Message));
            }

            return result;
        }
    }
}
