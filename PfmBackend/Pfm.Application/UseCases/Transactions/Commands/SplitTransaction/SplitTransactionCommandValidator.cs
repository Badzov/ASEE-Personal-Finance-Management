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
                .NotEmpty().WithErrorCode("required")
                .WithMessage("Transaction ID is required");

            RuleFor(x => x.Splits)
                .NotEmpty().WithErrorCode("required")
                .WithMessage("At least one split is required")
                .Must(splits => splits?.Select(s => s.CatCode).Distinct().Count() == splits?.Count)
                .WithErrorCode("duplicate-categories")
                .WithMessage("Cannot split into duplicate categories");

            RuleForEach(x => x.Splits)
                .ChildRules(split =>
                {
                    split.RuleFor(x => x.CatCode)
                        .NotEmpty().WithErrorCode("required")
                        .WithMessage("Category code is required");

                    split.RuleFor(x => x.Amount)
                        .GreaterThan(0).WithErrorCode("invalid-amount")
                        .WithMessage("Split amount must be positive");
                });
        }
    }
}
