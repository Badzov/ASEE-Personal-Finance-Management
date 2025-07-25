using Pfm.Domain.Entities;

namespace Pfm.Application.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetByCodesAsync(IEnumerable<string> codes);
    }
}
