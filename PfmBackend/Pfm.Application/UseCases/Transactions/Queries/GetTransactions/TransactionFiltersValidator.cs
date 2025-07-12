using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Transactions.Queries.GetTransactions
{
    public class TransactionFiltersValidator : AbstractValidator<TransactionFilters>
    {
        public TransactionFiltersValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be positive");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("End date must be after start date");

            RuleForEach(x => x.Kinds)
                .IsInEnum()
                .When(x => x.Kinds != null)
                .WithMessage("Invalid transaction kind value");
        }
    }
}
