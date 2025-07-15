using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Categories.Commands.ImportCategories
{
    public record ImportCategoriesDto(
        string Code,
        string? ParentCode,
        string Name
    );
}
