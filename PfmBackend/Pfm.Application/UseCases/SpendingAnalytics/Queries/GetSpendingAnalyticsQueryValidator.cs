using FluentValidation;
using Pfm.Application.UseCases.SpendingAnalytics.Queries;
using Pfm.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                .WithErrorCode("max-length")
                .WithMessage("Category code cannot exceed 10 characters")
                .Matches("^[A-Z0-9-]*$").When(x => !string.IsNullOrEmpty(x.CatCode))
                .WithErrorCode("invalid-format")
                .WithMessage("Category code must contain only uppercase letters, numbers and hyphens");


            RuleFor(x => x.StartDate)
                .Must(date => date != DateTime.MinValue)
                    .WithErrorCode("invalid-date")
                    .WithMessage("Invalid date formatting")
                .Must(date => !date.HasValue || date.Value.Year >= 2000)
                    .WithErrorCode("invalid-date")
                    .WithMessage("Start date must be from year 2000 onwards")
                .LessThanOrEqualTo(DateTime.Today)
                    .When(x => x.StartDate.HasValue)
                    .WithErrorCode("future-date")
                    .WithMessage("Start date cannot be in the future");

            RuleFor(x => x.EndDate)
                .Must(date => !date.HasValue || date.Value.Year >= 2000)
                    .WithErrorCode("invalid-date")
                    .WithMessage("End date must be from year 2000 onwards")
                .GreaterThanOrEqualTo(x => x.StartDate.GetValueOrDefault(DateTime.MinValue))
                    .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                    .WithErrorCode("invalid-range")
                    .WithMessage("End date must be after or equal to start date")
                .LessThanOrEqualTo(DateTime.Today)
                    .When(x => x.EndDate.HasValue)
                    .WithErrorCode("future-date")
                    .WithMessage("End date cannot be in the future");

            RuleFor(x => x.Direction)
                .Must(x => Enum.TryParse<DirectionsEnum>(x, out _)).When(x => !string.IsNullOrEmpty(x.Direction))
                .WithErrorCode("invalid-direction")
                .WithMessage("Direction must be 'd' (debit) or 'c' (credit)");

            RuleFor(x => x.Direction)
                .MaximumLength(1).When(x => !string.IsNullOrEmpty(x.Direction))
                .WithErrorCode("max-length")
                .WithMessage("Direction must be 1 character");
        }

    }
}
