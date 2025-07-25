using Microsoft.EntityFrameworkCore;
using Pfm.Domain.Entities;
using Pfm.Application.Interfaces;
using Pfm.Infrastructure.Persistence.DbContexts;

namespace Pfm.Infrastructure.Persistence.Repositories
{
    public class SplitRepository : Repository<SingleCategorySplit>, ISplitRepository
    {
        public SplitRepository(PfmDbContext context) : base(context) { }

        public Task DeleteByTransactionIdAsync(string transactionId)
        {
            return _context.Splits
                .Where(s => s.TransactionId == transactionId)
                .ExecuteDeleteAsync(); 
        }
    }
}
