using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.Common
{
    public record ImportResult<T>
    {
        public List<T> ValidRecords { get; init; } = new();
        public List<RecordError> Errors { get; init; } = new();
        public bool HasErrors => Errors.Any();
    }

    public record RecordError(string RecordId, string ErrorCode, string Message);
}
