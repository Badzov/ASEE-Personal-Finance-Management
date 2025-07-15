using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Api.Models.Problems
{
    public record BusinessProblem
    {
        public required string Problem { get; init; }
        public required string Message { get; init; }
        public string? Details { get; init; }
    }
}
