using Pfm.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Api.Schemas
{
    public record ApiError(

    string tag,
    string error,
    string message);

    public record ApiErrorResponse(IEnumerable<AppError> Errors);
}
