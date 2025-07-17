using Pfm.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Domain.Common
{
    public record PagedList<T>(
        int TotalCount,
        int PageSize,
        int Page,
        int TotalPages,
        SortOrderEnum SortOrder,
        string SortBy,
        List<T> Items
    );
}
