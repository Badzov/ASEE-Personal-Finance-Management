using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Transactions.Commands.CategorizeTransaction
{
    public class CategorizeTransactionCommandValidator : AbstractValidator<CategorizeTransactionCommand>
    {
        public CategorizeTransactionCommandValidator()
        {
            RuleFor(x => x.TransactionId)
                .NotEmpty().WithErrorCode("required").WithMessage("Transaction ID is required")
                .MaximumLength(8).WithErrorCode("max-length").WithMessage("Transaction ID cannot exceed 36 characters")
                .Matches("^[a-zA-Z0-9-]+$").WithErrorCode("invalid-format").WithMessage("Transaction ID can only contain alphanumeric characters and hyphens");

            RuleFor(x => x.CategoryCode)
                .NotEmpty().WithErrorCode("required").WithMessage("Category code is required")
                .MaximumLength(10).WithErrorCode("max-length").WithMessage("Category code can have max 10 characters")
                .Matches("^[A-Z0-9-]+$").WithErrorCode("invalid-format").WithMessage("Category code can only contain A-Z, 0-9, hyphens");
        }
    }
}
