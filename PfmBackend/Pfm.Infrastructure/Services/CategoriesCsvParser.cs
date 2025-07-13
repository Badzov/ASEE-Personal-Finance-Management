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
        public ImportResult<ImportCategoriesDto> Parse(Stream stream)
        {
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null, 
                TrimOptions = TrimOptions.Trim
            }); ;

            csv.Context.RegisterClassMap<CategoryCsvMap>();
            var records = new List<ImportCategoriesDto>();
            var errors = new List<RecordError>();

            try
            {
                while (csv.Read())
                {
                    try
                    {
                        records.Add(csv.GetRecord<ImportCategoriesDto>());
                    }
                    catch (CsvHelperException ex)
                    {
                        errors.Add(new RecordError(
                            csv.GetField("code") ?? "unknown",
                            "csv-parse-error",
                            ex.Message
                        ));
                    }
                }
            }
            catch (Exception ex) when (ex is HeaderValidationException || ex is ReaderException)
            {
                errors.Add(new RecordError(
                    "file",
                    "invalid-file",
                    "Invalid CSV format"
                ));
            }

            return new ImportResult<ImportCategoriesDto>
            {
                ValidRecords = records,
                Errors = errors
            };
        }
    }
}
