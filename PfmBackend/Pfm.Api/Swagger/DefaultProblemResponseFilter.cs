using Microsoft.OpenApi.Models;
using Pfm.Api.Models.Problems;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Api.Swagger
{
    public class DefaultProblemResponseFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Responses.TryAdd("default", new OpenApiResponse
            {
                Description = "Default",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new()
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(
                            typeof(DefaultProblem),
                            context.SchemaRepository)
                    }
                }
            });
        }
    }
}
