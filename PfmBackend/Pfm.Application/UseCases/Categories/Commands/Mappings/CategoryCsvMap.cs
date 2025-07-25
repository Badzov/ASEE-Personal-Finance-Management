using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Pfm.Application.UseCases.Categories.Commands.ImportCategories;
using Pfm.Application.UseCases.Transactions.Mappings;


namespace Pfm.Application.UseCases.Categories.Commands.Mappings
{
    public sealed class CategoryCsvMap : ClassMap<ImportCategoriesDto>
    {
        public CategoryCsvMap(Action<string, string, string> addError)
        {
            Map(m => m.Code).Name("code").TypeConverter(new TrimConverter());
            Map(m => m.ParentCode)
                .Name("parent-code")
                .TypeConverter(new SafeParentCodeConverter(addError))
                .Optional()
                .Default(null);
            Map(m => m.Name).Name("name").TypeConverter(new TrimConverter());
        }

        public class SafeParentCodeConverter : DefaultTypeConverter
        {
            private readonly Action<string, string, string> _addError;

            public SafeParentCodeConverter(Action<string, string, string> addError)
            {
                _addError = addError;
            }

            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                var code = row.GetField("code") ?? "unknown";

                if (string.IsNullOrWhiteSpace(text))
                    return null;

                var trimmed = text.Trim();

                return trimmed;
            }
        }
    }
}
