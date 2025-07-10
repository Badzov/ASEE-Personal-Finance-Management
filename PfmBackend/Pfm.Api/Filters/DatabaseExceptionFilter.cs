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
            switch (context.Exception)
            {
                case RecordNotFoundException ex:
                    context.Result = new NotFoundObjectResult(new
                    {
                        error = "not-found",
                        entityId = ex.EntityId,
                        message = ex.Message
                    });
                    break;

                case ConcurrentUpdateException:
                    context.Result = new ConflictObjectResult(new
                    {
                        error = "concurrency",
                        message = "The record was modified by another user. Please refresh and try again."
                    });
                    break;

                case DatabaseOperationException ex:
                    context.Result = new ObjectResult(new
                    {
                        error = "database-error",
                        operation = ex.OperationType,
                        message = ex.Message
                    })
                    {
                        StatusCode = 500
                    };
                    break;
            }

            if (context.Result != null)
                context.ExceptionHandled = true;
        }
    }
}
