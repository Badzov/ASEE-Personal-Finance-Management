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

                csv.Context.RegisterClassMap<CategoryCsvMap>();

                while (await csv.ReadAsync())
                {
                    try
                    {
                        var record = csv.GetRecord<ImportCategoriesDto>();
                        result.AddValidRecord(record);
                    }
                    catch (CsvHelperException ex)
                    {
                        result.AddError(
                            csv.GetField("code") ?? "unknown",
                            "csv-parse-error",
                            ex.Message
                        );
                    }
                }
            }
            catch (Exception ex) when (ex is HeaderValidationException || ex is ReaderException)
            {
                result.AddError("file", "invalid-file", "Invalid CSV format");
            }
            catch (Exception ex)
            {
                result.AddError("file", "processing-error", $"Error processing CSV: {ex.Message}");
            }

            return result;
        }
    }
}
