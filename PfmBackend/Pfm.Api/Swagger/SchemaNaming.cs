using Pfm.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pfm.Api.Swagger
{
    public static class SchemaNaming
    {
        public static string GetSchemaId(Type type)
        {
            if (!type.IsGenericType)
                return type.Name.ToKebabCase();

            if (type.GetGenericTypeDefinition() == typeof(PagedList<>))
            {
                var itemType = type.GetGenericArguments()[0];
                return $"{itemType.Name.ToKebabCase()}-paged-list";
            }

            return type.Name.ToKebabCase();
        }

        private static string ToKebabCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return Regex.Replace(
                input,
                "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
                "-$1",
                RegexOptions.Compiled)
                .Trim()
                .ToLower();
        }
    }
}
