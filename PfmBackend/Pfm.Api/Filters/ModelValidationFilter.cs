using Microsoft.AspNetCore.Mvc.Filters;
using Pfm.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pfm.Api.Filters
{
    public sealed class ModelValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var modelErrors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .SelectMany(e => e.Value!.Errors.Select(error => new ValidationError(
                CleanKey(e.Key),
                "ModelBinding",
                error.ErrorMessage)))
            .ToList();

            // Temporarily store errors in HttpContext so you can access them in your handler
            context.HttpContext.Items["ModelValidationErrors"] = modelErrors;

            await next();
        }

        static string CleanKey(string key)
        {
            return key
                .Replace("]", "")
                .Replace("[", "-")
                .Replace(".", "-")
                .ToLower();
        }
    }
}
