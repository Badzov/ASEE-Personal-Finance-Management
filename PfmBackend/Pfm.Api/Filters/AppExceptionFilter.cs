using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Pfm.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Api.Filters
{
    public class AppExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<AppExceptionFilter> _logger;

        public AppExceptionFilter(ILogger<AppExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is AppException appEx)
            {
                context.Result = new ObjectResult(new
                {
                    errors = appEx.Errors.Select(e => new
                    {
                        tag = e.Tag,
                        error = e.Error,
                        message = e.Message
                    })
                })
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            else
            {
                _logger.LogError(context.Exception, "Unhandled exception");

                context.Result = new ObjectResult(new
                {
                    errors = new[]
                    {
                        new
                        {
                            tag = "system",
                            error = "internal-error",
                            message = "An unexpected error occurred"
                        }
                    }
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
