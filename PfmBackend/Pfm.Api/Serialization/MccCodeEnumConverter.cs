using Pfm.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pfm.Api.Serialization
{
    public class MccCodeEnumConverter : JsonConverter<MccCodeEnum>
    {
        public override MccCodeEnum Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return (MccCodeEnum)reader.GetInt32();
            }
            return Enum.Parse<MccCodeEnum>(reader.GetString()!);
        }

        public override void Write(
            Utf8JsonWriter writer,
            MccCodeEnum value,
            JsonSerializerOptions options)
        {
            writer.WriteNumberValue((int)value); 
        }
    }
}
