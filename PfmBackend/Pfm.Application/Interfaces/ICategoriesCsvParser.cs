using Pfm.Application.Common;
using Pfm.Application.UseCases.Categories.Commands.ImportCategories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.Interfaces
{
    public interface ICategoriesCsvParser
    {
        ImportResult<ImportCategoriesDto> Parse(Stream stream);
    }
}
