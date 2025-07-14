using Microsoft.EntityFrameworkCore;
using Pfm.Domain.Entities;
using Pfm.Domain.Enums;
using Pfm.Domain.Interfaces;
using Pfm.Infrastructure.Persistence.DbContexts;

namespace Pfm.Infrastructure.Persistence.Repositories
{
    public class SpendingAnalysisRepository : Repository<SpendingAnalysis>, ISpendingAnalysisRepository
    {
        public SpendingAnalysisRepository(PfmDbContext context) : base(context) { }

        public async Task<IEnumerable<SpendingAnalysis>> GetFilteredAsync(string? catCode, DateTime? startDate, DateTime? endDate, TransactionDirection? direction)
        {
            var query = _context.Transactions.AsQueryable();

            if (!string.IsNullOrEmpty(catCode))
            {
                query = query.Where(t => t.CatCode.StartsWith(catCode));
            }

            if (startDate.HasValue)
            {
                query = query.Where(t => t.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.Date <= endDate.Value);
            }

            if (direction.HasValue)
            {
                query = query.Where(t => t.Direction == direction.Value);
            }

            return await query
                .GroupBy(t => t.CatCode)
                .Select(g => new SpendingAnalysis(
                    g.Key,
                    g.Sum(t => t.Amount),
                    g.Count()))
                .ToListAsync();
        }
    }
}
