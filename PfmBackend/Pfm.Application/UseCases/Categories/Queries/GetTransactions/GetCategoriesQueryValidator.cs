using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Categories.Queries.GetTransactions
{
    public class GetCategoriesQueryValidator : AbstractValidator<GetCategoriesQuery>
    {
        public GetCategoriesQueryValidator()
        {
            RuleFor(x => x.ParentCode)
            .MaximumLength(10).WithErrorCode("parentcode-length").WithMessage("Parent code can have max 10 characters")
            .Matches("^[A-Z0-9-]*$").WithErrorCode("parentcode-format").WithMessage("Parent code must contain only A-Z, 0-9, hyphens allowed")
            .When(x => !string.IsNullOrWhiteSpace(x.ParentCode));
        }
    }
}
