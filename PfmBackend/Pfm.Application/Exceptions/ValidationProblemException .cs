using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.Common
{
    public class ValidationProblemException : Exception
    {
        public IReadOnlyList<ValidationError> Errors { get; }

        public ValidationProblemException(IEnumerable<ValidationError> errors) : base("Validation failed")
        {
            Errors = errors.ToList().AsReadOnly();
        }
    }
}
