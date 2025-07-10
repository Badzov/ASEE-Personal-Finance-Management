using AutoMapper;
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
    public class DomainExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is DomainValidationException domEx)
            {
                context.Result = new ObjectResult(new
                {
                    errors = domEx.Errors.Select(e => new
                    {
                        tag = e.Tag,
                        error = e.Error,
                        message = e.Message
                    })
                })
                {
                    StatusCode = 422 
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
