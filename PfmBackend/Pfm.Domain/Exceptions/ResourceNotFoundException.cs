using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Domain.Exceptions
{
    public class ResourceNotFoundException : DomainProblemException
    {
        public ResourceNotFoundException(string resourceType, string resourceId)
            : base("resource-not-found",
                  $"{resourceType} with id {resourceId} not found",
                  $"Requested resource was not found")
        { }
    }
}
