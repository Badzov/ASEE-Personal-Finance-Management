using MediatR;
using Pfm.Domain.Entities;
using Pfm.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.SpendingAnalytics.Queries
{
    public record GetSpendingAnalyticsQuery(
        string? CatCode,
        DateTime? StartDate,
        DateTime? EndDate,
        string? Direction
    ) : IRequest<IEnumerable<SpendingAnalysisDto>>;
}
