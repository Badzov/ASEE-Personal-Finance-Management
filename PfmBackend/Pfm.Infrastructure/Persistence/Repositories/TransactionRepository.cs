using Microsoft.EntityFrameworkCore;
using Pfm.Domain.Entities;
using Pfm.Domain.Enums;
using Pfm.Domain.Interfaces;
using Pfm.Infrastructure.Persistence.DbContexts;

namespace Pfm.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(PfmDbContext context) : base(context) { }

        public async Task<(List<Transaction>, int)> GetFilteredAsync(
            DateTime? startDate,
            DateTime? endDate,
            IReadOnlyCollection<TransactionKind>? kinds,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = _context.Transactions.AsQueryable();

            if (startDate.HasValue) query = query.Where(t => t.Date >= startDate.Value);
            if (endDate.HasValue) query = query.Where(t => t.Date <= endDate.Value);

            if (kinds?.Count > 0)
            {
                query = query.Where(t => kinds.Contains(t.Kind));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var transactions = await query
                .OrderByDescending(t => t.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (transactions, totalCount);
        }
    }
}
