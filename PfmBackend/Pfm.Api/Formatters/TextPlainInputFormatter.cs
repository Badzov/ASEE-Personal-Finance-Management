using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Api.Formatters
{
    public class TextPlainInputFormatter : TextInputFormatter
    {
        public TextPlainInputFormatter()
        {
            SupportedMediaTypes.Add("text/csv");
            SupportedEncodings.Add(Encoding.UTF8);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding effectiveEncoding)
        {
            using var reader = new StreamReader(context.HttpContext.Request.Body, effectiveEncoding);
            var content = await reader.ReadToEndAsync();
            return await InputFormatterResult.SuccessAsync(content);
        }
    }
}
