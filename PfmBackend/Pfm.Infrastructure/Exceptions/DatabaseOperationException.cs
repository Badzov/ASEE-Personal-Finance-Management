using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Infrastructure.Exceptions
{
    public class DatabaseOperationException : Exception
    {
        public string OperationType { get; }

        public DatabaseOperationException(string operationType, string message) : base(message)
        {
            OperationType = operationType;
        }
    }

    public class ConcurrentUpdateException : DatabaseOperationException
    {
        public ConcurrentUpdateException() : base("update", "Concurrent modification detected") { }
    }

    public class RecordNotFoundException : DatabaseOperationException
    {
        public string EntityId { get; }

        public RecordNotFoundException(string entityType, string entityId) : base("fetch", $"{entityType} with id {entityId} not found")
        {
            EntityId = entityId;
        }
    }
}
