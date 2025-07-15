using CsvHelper;
using Pfm.Application.Common;
using Pfm.Application.Interfaces;
using Pfm.Application.UseCases.Transactions.Commands.ImportTransactions;
using Pfm.Application.UseCases.Transactions.Mappings;
using System.Globalization;

namespace Pfm.Infrastructure.Services
{
    public class TransactionCsvParser : ITransactionsCsvParser
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
                        result.AddError(recordId, "required", "Transaction ID is required");
                        continue;
                    }

                    result.AddValidRecord(record);
                }
            }
            catch (CsvHelperException ex)
            {
                result.AddError("file", "invalid-csv", ex.Message);
            }
            catch (Exception ex)
            {
                result.AddError("file", "processing-error", $"Error processing CSV: {ex.Message}");
            }

            return result;
        }
    }
}
