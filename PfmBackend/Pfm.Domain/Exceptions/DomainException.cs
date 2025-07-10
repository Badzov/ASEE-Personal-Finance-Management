using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public string ErrorCode { get; }

        public DomainException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
