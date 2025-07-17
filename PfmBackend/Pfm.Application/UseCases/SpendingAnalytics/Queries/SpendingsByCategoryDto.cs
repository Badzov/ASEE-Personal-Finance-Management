using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.SpendingAnalytics.Queries
{
    public record SpendingsByCategoryDto(
        List<SpendingInCategoryDto> Groups
    );
}
