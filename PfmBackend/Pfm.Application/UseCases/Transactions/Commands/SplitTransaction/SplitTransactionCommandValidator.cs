using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Transactions.Commands.SplitTransaction
{
    public class SplitTransactionCommandValidator : AbstractValidator<SplitTransactionCommand>
    {
        public SplitTransactionCommandValidator()
        {
            RuleFor(x => x.TransactionId)
                .NotEmpty().WithErrorCode("required").WithMessage("Transaction ID is required")
                .MaximumLength(8).WithErrorCode("max-length").WithMessage("Transaction ID cannot exceed 36 characters")
                .Matches("^[a-zA-Z0-9-]+$").WithErrorCode("invalid-format").WithMessage("Transaction ID can only contain alphanumeric characters and hyphens");

            RuleFor(x => x.Splits)
                .NotEmpty().WithErrorCode("required")
                .WithMessage("At least one split is required")
                .Must(splits => splits != null && splits.Count >= 2)
                .WithErrorCode("invalid-split-count")
                .WithMessage("At least two splits are required")
                .Must(splits => splits?.Select(s => s.CatCode).Distinct().Count() == splits?.Count)
                .WithErrorCode("duplicate-categories")
                .WithMessage("Cannot split into duplicate categories");

            RuleForEach(x => x.Splits)
                .ChildRules(split =>
                {
                    split.RuleFor(x => x.CatCode)
                        .NotEmpty().WithErrorCode("required").WithMessage("Category code is required")
                        .MaximumLength(10).WithErrorCode("max-length").WithMessage("Category code can have max 10 characters")
                        .Matches("^[A-Z0-9-]+$").WithErrorCode("invalid-format").WithMessage("Category code can only contain A-Z, 0-9, hyphens");

                    split.RuleFor(x => x.Amount)
                        .GreaterThan(0).WithErrorCode("invalid-amount")
                        .WithMessage("Split amount must be positive");
                });
        }
    }
}
