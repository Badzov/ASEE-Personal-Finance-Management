using Pfm.Domain.Entities;

namespace Pfm.Application.Interfaces
{
    public interface ISplitRepository : IRepository<SingleCategorySplit>
    {
        Task DeleteByTransactionIdAsync(string transactionId);
    }
}
