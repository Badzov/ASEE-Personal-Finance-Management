using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Domain.Exceptions
{
    public class BusinessRuleException : DomainProblemException
    {
        public BusinessRuleException(string problemCode, string message, string details = null): base(problemCode, message, details) { }
    }
}
