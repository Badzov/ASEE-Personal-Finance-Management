using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Infrastructure.Exceptions
{
    public class PersistenceException : Exception
    {
        public string OperationType { get; }

        public PersistenceException(string operationType, string message, Exception inner = null) : base(message, inner)
        {
            OperationType = operationType;
        }
    }
}
