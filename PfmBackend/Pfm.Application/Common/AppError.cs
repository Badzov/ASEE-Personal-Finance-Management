using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.Common
{
    public record AppError(string Tag, string Error, string Message);
}
