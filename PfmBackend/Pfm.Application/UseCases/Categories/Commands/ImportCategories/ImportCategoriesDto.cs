using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Categories.Commands.ImportCategories
{
    public class ImportCategoriesDto {
        public string Code { get; set; }
        public string? ParentCode { get; set; }
        public string Name { get; set; }
    };
}
