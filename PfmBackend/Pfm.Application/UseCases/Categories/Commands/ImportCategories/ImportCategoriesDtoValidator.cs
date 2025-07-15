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
                .NotEmpty().WithErrorCode("required").WithMessage("Code is required")
                .MaximumLength(10).WithErrorCode("max-length").WithMessage("Category code cannot exceed 10 characters")
                .Matches("^[A-Z0-9-]+$").WithErrorCode("invalid-format").WithMessage("Only uppercase letters, numbers and hyphens allowed");

            RuleFor(x => x.Name)
                .NotEmpty().WithErrorCode("required").WithMessage("Name is required");

            RuleFor(x => x.ParentCode)
                .MaximumLength(10).WithErrorCode("max-length").WithMessage("Parent code cannot exceed 10 characters")
                .Matches("^[A-Z0-9-]*$").WithErrorCode("invalid-format").WithMessage("Only uppercase letters, numbers and hyphens allowed");
        }
    }
}
