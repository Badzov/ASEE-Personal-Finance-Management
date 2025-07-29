using Microsoft.Data.SqlClient;
using Pfm.Domain.Entities;
using Pfm.Domain.Enums;
using System.Data;

namespace Pfm.Application.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<(IEnumerable<Transaction> Transactions, int TotalCount)> GetFilteredAsync(
            DateTime? startDate,
            DateTime? endDate,
            IReadOnlyCollection<string>? kinds,
            string? sortBy,
            string? sortOrder,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);

        Task<IEnumerable<Transaction>> GetFilteredAsync(
            DateTime? startDate,
            DateTime? endDate,
            string? catCode,
            DirectionsEnum? direction,
            CancellationToken cancellationToken);

        Task<Transaction> GetByIdWithSplitsAsync(string id);

        Task<int> CountUncategorizedAsync(CancellationToken ct);

        Task<int> ExecuteUpdateAsync(string sql, List<IDbDataParameter> parameters);
    }
}
