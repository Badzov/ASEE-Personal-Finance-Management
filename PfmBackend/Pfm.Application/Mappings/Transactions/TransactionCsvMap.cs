using AutoMapper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper;
using Pfm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Pfm.Application.DTOs.Requests;

namespace Pfm.Application.Mappings.Transactions
{
    public sealed class TransactionCsvMap : ClassMap<TransactionCsvRequest>
    {
        public TransactionCsvMap()
        {
            Map(m => m.Id).Name("id");
            Map(m => m.BeneficiaryName).Name("beneficiary-name");
            Map(m => m.Date).Name("date").TypeConverter<DateTimeConverter>();
            Map(m => m.Direction).Name("direction");
            Map(m => m.Amount).Name("amount").TypeConverter<CurrencyConverter>();
            Map(m => m.Description).Name("description");
            Map(m => m.Currency).Name("currency").TypeConverter<TrimConverter>();
            Map(m => m.Mcc).Name("mcc").Optional();
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

    public class CurrencyConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0m;

            var cleanValue = text.Replace("€", "").Replace(",", "").Trim();
            return decimal.Parse(cleanValue, CultureInfo.InvariantCulture);
        }
    }

    public class DateTimeConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            return DateTime.ParseExact(text, "M/d/yyyy", CultureInfo.InvariantCulture);
        }
    }
}
