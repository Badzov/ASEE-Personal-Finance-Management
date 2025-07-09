using Pfm.Domain.Interfaces;
using Pfm.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pfm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Pfm.Infrastructure.Repositories
{
    public class TransactionRepository : IRepository<Transaction>
    {
        private readonly PfmDbContext _context;
        public TransactionRepository(PfmDbContext context) => _context = context;

        public async Task AddAsync(Transaction entity)
        {
            await _context.Transactions.AddAsync(entity);
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<Transaction> GetByIdAsync(string id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task UpdateAsync(Transaction entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var transaction = await GetByIdAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
        }
    }
}
