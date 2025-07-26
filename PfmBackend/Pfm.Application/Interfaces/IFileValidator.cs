using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.Interfaces
{
    public interface IFileValidator
    {
        public void Validate(IFormFile file);
    }
}
