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
                .NotEmpty().WithMessage("Code is required")
                .MaximumLength(10).WithMessage("Category code cannot exceed 10 characters")
                .Matches("^[A-Z0-9-]+$").WithMessage("Only uppercase letters, numbers and hyphens allowed");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required");
        }
    }
}
