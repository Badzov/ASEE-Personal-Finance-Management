using CsvHelper;
using CsvHelper.Configuration;
using Pfm.Application.Common;
using Pfm.Application.Interfaces;
using Pfm.Application.UseCases.Categories.Commands.ImportCategories;
using Pfm.Application.UseCases.Categories.Commands.Mappings;
using System.Globalization;


namespace Pfm.Infrastructure.Services
{
    public class CategoriesCsvParser : ICategoriesCsvParser
    {
        public async Task<ImportResult<ImportCategoriesDto>> ParseAsync(Stream stream)
        {
            var result = new ImportResult<ImportCategoriesDto>();

            try
            {
                using var reader = new StreamReader(stream);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound = null,
                    TrimOptions = TrimOptions.Trim
                });

                csv.Context.RegisterClassMap(new CategoryCsvMap((tag, code, message) =>
                {
                    result.AddError(tag, code, message);
                }));

                await foreach (var record in csv.GetRecordsAsync<ImportCategoriesDto>())
                {
                    var recordId = record.Code ?? "unknown";

                    if (string.IsNullOrWhiteSpace(record.Code))
                    {
                        result.AddError(recordId, "required", "Category code is required");
                        continue;
                    }

                    result.AddValidRecord(record);
                }
            }
            catch (Exception ex) when (ex is HeaderValidationException || ex is ReaderException)
            {
                result.AddError("csv", "invalid-csv", "Invalid CSV format");
            }
            catch (CsvHelperException ex)
            {
                result.AddError("csv", "invalid-csv", ex.Message);
            }
            catch (Exception ex)
            {
                result.AddError("csv", "processing-error", $"Error processing CSV: {ex.Message}");
            }

            return result;
        }
    }
}
