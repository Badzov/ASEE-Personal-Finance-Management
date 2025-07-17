using FluentValidation;
using Pfm.Application.UseCases.SpendingAnalytics.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Queries
{
    public class GetSpendingAnalyticsQueryValidator : AbstractValidator<GetSpendingAnalyticsQuery>
    {
        public GetSpendingAnalyticsQueryValidator()
        {
            RuleFor(x => x.CatCode)
                .MaximumLength(10).When(x => !string.IsNullOrEmpty(x.CatCode))
                .WithMessage("Category code cannot exceed 10 characters");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("End date must be after start date");

            RuleFor(x => x.Direction)
                .Must(d => d == null || d.ToLower() == "d" || d.ToLower() == "c")
                .WithMessage("Direction must be 'd' (debit) or 'c' (credit)");
        }
    }
}
