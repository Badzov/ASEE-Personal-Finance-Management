using Pfm.Domain.Interfaces;
using Pfm.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pfm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Pfm.Infrastructure.Exceptions;
using System.Data.SqlClient;

namespace Pfm.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly PfmDbContext _context;
        private readonly string _entityName = typeof(T).Name;

        public Repository(PfmDbContext context)
        {
            _context = context;
        }



        public async Task AddAsync(T entity)
        {
            try
            {
                await _context.Set<T>().AddAsync(entity);
            }
            catch (DbUpdateException ex) when (IsConcurrencyConflict(ex))
            {
                throw new ConcurrentUpdateException();
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseOperationException("insert", ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        public async Task<T> GetByIdAsync(string id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            return entity ?? throw new RecordNotFoundException(_entityName, id);
        }
        public async Task UpdateAsync(T entity)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrentUpdateException();
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseOperationException("update", ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null) _context.Set<T>().Remove(entity);
        }



        private static bool IsConcurrencyConflict(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601); 
        }
    }
}
