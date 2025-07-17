using FluentValidation;
using Pfm.Domain.Enums;
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

            RuleFor(x => x.Kinds)
                .Must(BeValidTransactionKinds)
                .When(x => x.Kinds != null)
                .WithMessage("Invalid transaction kind value(s)");

            RuleFor(x => x.SortBy)
                .Must(BeAValidSortField)
                .When(x => !string.IsNullOrEmpty(x.SortBy))
                .WithMessage("Invalid sort field");

            RuleFor(x => x.SortOrder)
                .Must(so => so == null || so.ToLower() == "asc" || so.ToLower() == "desc")
                .WithMessage("Sort order must be 'asc' or 'desc'");
        }

        private bool BeValidTransactionKinds(IReadOnlyCollection<string>? kindStrings)
        {
            if (kindStrings == null) return true;

            foreach (var kind in kindStrings)
            {
                if (!Enum.TryParse<TransactionKindsEnum>(kind, ignoreCase: true, out _))
                {
                    return false;
                }
            }
            return true;
        }

        private bool BeAValidSortField(string? sortBy)
        {
            if (string.IsNullOrEmpty(sortBy)) return true;

            var validFields = new[] { "date", "amount", "cat-code", "kind", "direction", "beneficiary-name" };
            return validFields.Contains(sortBy.ToLower());
        }
    }
}
