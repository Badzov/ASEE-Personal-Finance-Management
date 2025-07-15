using Pfm.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Api.Models.Problems
{
    public record ValidationProblem
    {
        public required IReadOnlyList<ValidationError> Errors { get; init; }
    }
}
