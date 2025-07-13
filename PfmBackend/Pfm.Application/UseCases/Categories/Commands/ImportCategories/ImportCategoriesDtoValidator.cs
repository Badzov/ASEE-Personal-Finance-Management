using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Categories.Commands.ImportCategories
{
    public class ImportCategoriesDtoValidator : AbstractValidator<ImportCategoriesDto>
    {
        public ImportCategoriesDtoValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required");
        }
    }
}
