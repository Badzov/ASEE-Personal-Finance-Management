using Pfm.Domain.Entities;
using Pfm.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Domain.Interfaces
{
    public interface ISpendingAnalysisRepository : IRepository<SpendingAnalysis>
    {
        Task<IEnumerable<SpendingAnalysis>> GetFilteredAsync(
            string? catCode,
            DateTime? startDate,
            DateTime? endDate,
            TransactionDirection? direction);
    }
}
