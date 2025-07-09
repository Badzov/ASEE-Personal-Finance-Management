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
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly PfmDbContext _context;

        public Repository(PfmDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }
        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();
        public async Task<T> GetByIdAsync(string id) => await _context.Set<T>().FindAsync(id);
        public async Task UpdateAsync(T entity) => _context.Entry(entity).State = EntityState.Modified;
        public async Task DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null) _context.Set<T>().Remove(entity);
        }
    }
}
