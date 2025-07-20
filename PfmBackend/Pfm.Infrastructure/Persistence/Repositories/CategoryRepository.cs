using Microsoft.EntityFrameworkCore;
using Pfm.Domain.Entities;
using Pfm.Domain.Interfaces;
using Pfm.Infrastructure.Persistence.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
