using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Api.Models.Problems
{
    public record DefaultProblem
    {
        public required string Title { get; init; }
        public required int Status { get; init; }
        public string? Details { get; init; }
    }
}
