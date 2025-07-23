using Pfm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pfm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Pfm.Infrastructure.Exceptions;
using System.Data.SqlClient;
using Pfm.Infrastructure.Persistence.DbContexts;

namespace Pfm.Infrastructure.Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        internal readonly PfmDbContext _context;
        private readonly string _entityName = typeof(T).Name;
        protected readonly DbSet<T> _dbSet;

        public Repository(PfmDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
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
                throw new PersistenceException("insert", ex.InnerException?.Message ?? ex.Message);
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
            return entity;
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
                throw new PersistenceException("update", ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null) _context.Set<T>().Remove(entity);
        }

        public async Task<bool> ExistsAsync(string id, CancellationToken ct = default)
        {
            // For entities with string keys named "Code"
            if (typeof(T).GetProperty("Code") != null)
            {
                return await _dbSet.AnyAsync(e => EF.Property<string>(e, "Code") == id, ct);
            }
            // For entities with string keys named "Id"
            else if (typeof(T).GetProperty("Id") != null)
            {
                return await _dbSet.AnyAsync(e => EF.Property<string>(e, "Id") == id, ct);
            }

            throw new NotSupportedException($"Entity {typeof(T).Name} doesn't have a supported key property");
        }



        private static bool IsConcurrencyConflict(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601);
        }
    }
}
