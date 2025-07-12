using Pfm.Domain.Entities;
using Pfm.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Domain.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<(List<Transaction> Transactions, int TotalCount)> GetFilteredAsync(
            DateTime? startDate,
            DateTime? endDate,
            IReadOnlyCollection<TransactionKind>? kinds,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);
    }
}
