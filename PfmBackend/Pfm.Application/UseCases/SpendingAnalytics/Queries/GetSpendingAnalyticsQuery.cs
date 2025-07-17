using MediatR;
using Pfm.Application.UseCases.SpendingAnalytics.Queries;

namespace Pfm.Application.UseCases.Queries
{
    public record GetSpendingAnalyticsQuery(
        string? CatCode,
        DateTime? StartDate,
        DateTime? EndDate,
        string? Direction
    ) : IRequest<SpendingsByCategoryDto>;
}
