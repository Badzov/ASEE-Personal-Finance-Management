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
                .NotEmpty().WithErrorCode("required").WithMessage("Code required")
                .MaximumLength(10).WithErrorCode("max-length").WithMessage("Code can have max 10 characters")
                .Matches("^[A-Z0-9-]+$").WithErrorCode("invalid-format").WithMessage("Code must contain only A-Z, 0-9, hyphens allowed");

            RuleFor(x => x.Name)
                .NotEmpty().WithErrorCode("required").WithMessage("Name required");

            RuleFor(x => x.ParentCode)
                .MaximumLength(10).WithErrorCode("max-length").WithMessage("Parent code can have max 10 characters")
                .Matches("^[A-Z0-9-]*$").WithErrorCode("invalid-format").WithMessage("Parent code must contain only A-Z, 0-9, hyphens allowed");
        }
    }
}
