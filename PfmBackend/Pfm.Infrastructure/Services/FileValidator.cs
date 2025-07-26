using Microsoft.AspNetCore.Http;
using Pfm.Application.Common;
using Pfm.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Infrastructure.Services
{
    public class FileValidator : IFileValidator
    {
        public void Validate(IFormFile file)
        {
            var errors = new List<ValidationError>();

            if (file == null || file.Length == 0)
                errors.Add(new ValidationError("file", "required", "File is required"));

            if (file?.Length > 5 * 1024 * 1024)
                errors.Add(new ValidationError("file", "too-large", "Max file size is 5MB"));

            if (file != null && Path.GetExtension(file.FileName).ToLower() != ".csv")
                errors.Add(new ValidationError("file", "invalid-type", "Only CSV files allowed"));

            if (file != null && file.FileName != Path.GetFileName(file.FileName))
                errors.Add(new ValidationError("file", "invalid-name", "Invalid filename"));

            if (errors.Any())
                throw new ValidationProblemException(errors);
        }
    }
}
