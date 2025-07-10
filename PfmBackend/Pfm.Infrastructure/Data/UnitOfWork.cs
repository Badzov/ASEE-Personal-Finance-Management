using Microsoft.EntityFrameworkCore;
using Pfm.Domain.Entities;
using Pfm.Domain.Interfaces;
using Pfm.Infrastructure.Exceptions;
using Pfm.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PfmDbContext _context;

        public IRepository<Transaction> Transactions { get; }
        public IRepository<Category> Categories { get; }
        public IRepository<Split> Splits { get; }

        public UnitOfWork(PfmDbContext context)
        {
            _context = context;
            Transactions = new Repository<Transaction>(_context);
            Categories = new Repository<Category>(_context);
            Splits = new Repository<Split>(_context);
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
                throw new DatabaseOperationException("save", ex.InnerException?.Message ?? ex.Message);
            }
        }

        public void Dispose() => _context.Dispose();
    }
}
