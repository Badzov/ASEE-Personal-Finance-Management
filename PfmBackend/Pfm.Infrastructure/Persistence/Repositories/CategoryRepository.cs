using Microsoft.EntityFrameworkCore;
using Pfm.Domain.Entities;
using Pfm.Application.Interfaces;
using Pfm.Infrastructure.Persistence.DbContexts;
using Pfm.Infrastructure;

namespace Pfm.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(PfmDbContext context) : base(context) { }

        public async Task<IEnumerable<Category>> GetByCodesAsync(IEnumerable<string> codes)
        {
            return await _context.Categories
                .Where(c => codes.Contains(c.Code))
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetByParentCodeAsync(string parentCode)
        {
            return await _context.Categories
                .Where(c => c.ParentCode == parentCode)
                .ToListAsync();
        }
    }
}
