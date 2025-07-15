using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Pfm.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Api.Filters
{
    public class DatabaseExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is PersistenceException persistenceEx)
            {
                var problem = new ProblemDetails
                {
                    Title = "Database operation failed",
                    Detail = persistenceEx.Message,
                    Status = persistenceEx switch
                    {
                        ConcurrentUpdateException => StatusCodes.Status409Conflict,
                        RecordNotFoundException => StatusCodes.Status404NotFound,
                        _ => StatusCodes.Status500InternalServerError
                    }
                };

                context.Result = new ObjectResult(problem) { StatusCode = problem.Status };
                context.ExceptionHandled = true;
            }
        }
    }
}
