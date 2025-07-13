using CsvHelper.Configuration;
using Pfm.Application.UseCases.Categories.Commands.ImportCategories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Categories.Commands.Mappings
{
    public sealed class CategoryCsvMap : ClassMap<ImportCategoriesDto>
    {
        public CategoryCsvMap()
        {
            Map(m => m.Code).Name("code");
            Map(m => m.ParentCode).Name("parent-code").Optional().Default(null);
            Map(m => m.Name).Name("name");
        }
    }
}
