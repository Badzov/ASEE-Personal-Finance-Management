using Pfm.Api.Models.Problems;
using Pfm.Api.Serialization;
using Pfm.Application.Common;
using Pfm.Domain.Exceptions;
using Pfm.Infrastructure.Exceptions;
using System.Text.Json;

namespace Pfm.Api.Middleware
{
    public class ProblemDetailsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ProblemDetailsMiddleware> _logger;

        public ProblemDetailsMiddleware(RequestDelegate next, ILogger<ProblemDetailsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = exception switch
            {
                ValidationProblemException valEx => CreateValidationProblem(valEx),
                BusinessRuleException bizEx => CreateBusinessProblem(bizEx),
                PersistenceException persEx => CreateDatabaseProblem(persEx),
                _ => CreateDefaultProblem(exception)
            };

            if (exception is not (ValidationProblemException or BusinessRuleException or PersistenceException))
            {
                _logger.LogError(exception, "Unhandled exception occurred");
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = response.StatusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(response.Problem, JsonSerializationOptions.Default));
        }


        private static (object Problem, int StatusCode) CreateValidationProblem(ValidationProblemException exception)
        {
            return (new ValidationProblem
            {
                Errors = exception.Errors.Select(e => new ValidationError(e.Tag, e.Error, e.Message)).ToList()
            }, StatusCodes.Status400BadRequest);
        }

        private static (object Problem, int StatusCode) CreateBusinessProblem(BusinessRuleException exception)
        {
            return (new BusinessProblem
            {
                Problem = exception.ProblemCode,
                Message = exception.Message,
                Details = exception.Details
            }, 440); 
        }

        private static (object Problem, int StatusCode) CreateDatabaseProblem(PersistenceException exception)
        {
            return (new DefaultProblem
            {
                Title = "Database operation failed",
                Status = exception switch
                {
                    ConcurrentUpdateException => StatusCodes.Status409Conflict,
                    _ => StatusCodes.Status500InternalServerError
                },
                Details = exception.Message
            }, exception switch
            {
                ConcurrentUpdateException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            });
        }

        private static (object Problem, int StatusCode) CreateDefaultProblem(Exception exception)
        {
            return (new DefaultProblem
            {
                Title = "An unexpected error occurred",
                Status = StatusCodes.Status500InternalServerError,
                Details = exception.Message
            }, StatusCodes.Status500InternalServerError);
        }
    }
}
