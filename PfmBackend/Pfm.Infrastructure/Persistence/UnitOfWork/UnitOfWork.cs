using Microsoft.EntityFrameworkCore;
using Pfm.Application.Interfaces;
using Pfm.Infrastructure.Exceptions;
using Pfm.Infrastructure.Persistence.DbContexts;
using Pfm.Infrastructure.Persistence.Repositories;

namespace Pfm.Infrastructure.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PfmDbContext _context;
        public ITransactionRepository Transactions { get; }
        public ICategoryRepository Categories { get; }
        public ISplitRepository Splits { get; }

        public UnitOfWork(PfmDbContext context)
        {
            _context = context;
            Transactions = new TransactionRepository(_context);
            Categories = new CategoryRepository(_context);
            Splits = new SplitRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrentUpdateException();
            }
            catch (DbUpdateException ex)
            {
                throw new PersistenceException("save", ex.InnerException?.Message ?? ex.Message);
            }
        }

        public void Dispose() => _context.Dispose();
    }
}
