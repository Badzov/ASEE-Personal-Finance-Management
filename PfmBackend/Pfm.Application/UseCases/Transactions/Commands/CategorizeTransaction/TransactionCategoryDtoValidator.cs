using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Transactions.Commands.CategorizeTransaction
{
    public class TransactionCategoryDtoValidator : AbstractValidator<TransactionCategoryDto>
    {
        public TransactionCategoryDtoValidator()
        {
            RuleFor(x => x.CategoryCode)
                .NotEmpty().WithMessage("Category code is required")
                .MaximumLength(10).WithMessage("Category code cannot exceed 10 characters")
                .Matches("^[A-Z0-9-]+$").WithMessage("Only uppercase letters, numbers and hyphens allowed");
        }
    }
}
