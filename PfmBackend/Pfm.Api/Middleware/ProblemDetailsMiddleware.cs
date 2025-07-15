using Pfm.Api.Models.Problems;
using Pfm.Application.Common;
using Pfm.Domain.Exceptions;
using System.Text.Json;

namespace Pfm.Api.Middleware
{
    public class ProblemDetailsMiddleware
    {
        private readonly RequestDelegate _next;

        public ProblemDetailsMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var problem = exception switch
            {
                ValidationProblemException valEx => CreateValidationProblem(valEx),
                BusinessRuleException bizEx => CreateBusinessProblem(bizEx),
                _ => null 
            };

            if (problem != null)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            }

            return Task.CompletedTask;
        }

        private static (object Value, int Status)? CreateValidationProblem(ValidationProblemException exception)
        {
            return (new ValidationProblem
            {
                Errors = exception.Errors.Select(e => new ValidationError(e.Tag, e.Error, e.Message)).ToList()
            }, StatusCodes.Status400BadRequest);
        }

        private static (object Value, int Status)? CreateBusinessProblem(BusinessRuleException exception)
        {
            return (new BusinessProblem
            {
                Problem = exception.ProblemCode,
                Message = exception.Message,
                Details = exception.Details
            }, 440); 
        }
    }
}
