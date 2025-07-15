using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.Common
{
    public class ImportResult<T>
    {
        public List<T> ValidRecords { get; } = new();
        public IReadOnlyList<ValidationError> Errors { get; }
        public bool HasErrors => Errors.Any();

        private readonly List<ValidationError> _errors = new();

        public ImportResult()
        {
            Errors = _errors.AsReadOnly();
        }

        public void AddValidRecord(T record) => ValidRecords.Add(record);

        public void AddError(string tag, string error, string message)
        {
            _errors.Add(new ValidationError(tag, error, message));
        }

        public void AddError(ValidationError error)
        {
            _errors.Add(error);
        }

        public void ThrowIfErrors()
        {
            if (HasErrors)
            {
                throw new ValidationProblemException(Errors);
            }
        }
    }
}
