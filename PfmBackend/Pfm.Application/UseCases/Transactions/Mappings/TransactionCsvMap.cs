using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper;
using System.Globalization;
using Pfm.Application.UseCases.Transactions.Commands.ImportTransactions;
using Pfm.Application.Common;
using Pfm.Domain.Enums;

namespace Pfm.Application.UseCases.Transactions.Mappings
{
    public sealed class TransactionCsvMap : ClassMap<ImportTransactionsDto>
    {
        public TransactionCsvMap(Action<string, string, string> addError)
        {
            Map(m => m.Id).Name("id");
            Map(m => m.BeneficiaryName).Name("beneficiary-name");
            Map(m => m.Date).Name("date").TypeConverter(new SafeDateTimeConverter(addError));
            Map(m => m.Direction).Name("direction");
            Map(m => m.Amount).Name("amount").TypeConverter(new SafeCurrencyConverter(addError));
            Map(m => m.Description).Name("description");
            Map(m => m.Currency).Name("currency").TypeConverter(new TrimConverter());
            Map(m => m.Mcc).Name("mcc").TypeConverter(new SafeMccConverter(addError)).Optional();
            Map(m => m.Kind).Name("kind");
        }
    }

    public class TrimConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            return text?.Trim();
        }
    }

    public class SafeCurrencyConverter : DefaultTypeConverter
    {
        private readonly Action<string, string, string> _addError;

        public SafeCurrencyConverter(Action<string, string, string> addError)
        {
            _addError = addError;
        }

        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            var id = row.GetField("id") ?? "unknown";
            if (string.IsNullOrWhiteSpace(text)) return 0d;

            var cleanValue = text.Replace("€", "").Replace(",", "").Trim();

            if (double.TryParse(cleanValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;

            _addError(id, "invalid-amount-format", $"Invalid amount format: '{text}'");
            return 0d;
        }
    }

    public class SafeDateTimeConverter : DefaultTypeConverter
    {
        private readonly Action<string, string, string> _addError;

        public SafeDateTimeConverter(Action<string, string, string> addError)
        {
            _addError = addError;
        }

        private static readonly string[] formats = new[]
        {
            "M/d/yyyy",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-dd"
        };

        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            var id = row.GetField("id") ?? "unknown";
            if (DateTime.TryParseExact(text, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt;

            _addError(id, "invalid-date-format", $"Invalid date format: '{text}'");
            return default;
        }
    }
    public class SafeMccConverter : DefaultTypeConverter
    {
        private readonly Action<string, string, string> _addError;

        public SafeMccConverter(Action<string, string, string> addError)
        {
            _addError = addError;
        }

        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            var id = row.GetField("id") ?? "unknown";

            if (string.IsNullOrWhiteSpace(text))
                return null;

            if (!int.TryParse(text, out var mcc))
            {
                _addError(id, "invalid-mcc-format", $"MCC value '{text}' is not a number");
                return null;
            }

            return mcc;
        }
    }
}
