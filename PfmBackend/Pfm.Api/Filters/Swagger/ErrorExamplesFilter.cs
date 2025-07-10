using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Api.Filters.Swagger
{
    public class ErrorExamplesFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Responses.TryGetValue("400", out var badRequest))
            {
                badRequest.Content["application/json"].Example = new OpenApiObject
                {
                    ["errors"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["tag"] = new OpenApiString("record-123"),
                        ["error"] = new OpenApiString("invalid-format"),
                        ["message"] = new OpenApiString("Invalid CSV format")
                    }
                }
                };
            }

            if (operation.Responses.TryGetValue("422", out var unprocessable))
            {
                unprocessable.Content["application/json"].Example = new OpenApiObject
                {
                    ["errors"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["tag"] = new OpenApiString("record-456"),
                        ["error"] = new OpenApiString("invalid-amount"),
                        ["message"] = new OpenApiString("Amount must be positive")
                    }
                }
                };
            }
        }
    }
}
