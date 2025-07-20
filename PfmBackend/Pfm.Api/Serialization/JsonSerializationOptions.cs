using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pfm.Api.Serialization
{
    public static class JsonSerializationOptions
    {
        public static JsonSerializerOptions Default => new()
        {
            PropertyNamingPolicy = new KebabCaseNamingPolicy(),
            Converters =
            {
                new JsonStringEnumConverter(), 
                new MccCodeEnumConverter()     
            },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }
}
