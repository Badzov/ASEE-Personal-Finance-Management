using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Domain.Exceptions
{
    public abstract class DomainProblemException : Exception
    {
        public string ProblemCode { get; }
        public string? Details { get; }

        protected DomainProblemException(string problemCode, string message, string? details = null) : base(message)
        {
            ProblemCode = problemCode;
            Details = details;
        }
    }
}
