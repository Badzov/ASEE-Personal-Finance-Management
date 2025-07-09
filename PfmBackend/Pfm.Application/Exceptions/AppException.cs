using Pfm.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.Exceptions
{
    public class AppException : Exception
    {
        public List<AppError> Errors { get; }

        public AppException(List<AppError> errors, string message) : base(message)
        {
            Errors = errors;
        }
           
    }
}
