using Pfm.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.Exceptions
{
    public class DomainValidationException : Exception
    {
        public List<AppError> Errors { get; }

        public DomainValidationException(List<AppError> errors) : base("Domain validation failed")
        {
            Errors = errors;
        }
    }
}
