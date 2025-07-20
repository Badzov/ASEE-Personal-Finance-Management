using Microsoft.EntityFrameworkCore;
using Pfm.Domain.Entities;
using Pfm.Domain.Enums;
using Pfm.Domain.Interfaces;
using Pfm.Infrastructure.Exceptions;
using Pfm.Infrastructure.Persistence.DbContexts;

namespace Pfm.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(PfmDbContext context) : base(context) { }

        public async Task<(IEnumerable<Transaction> Transactions, int TotalCount)> GetFilteredAsync(
            DateTime? startDate,
            DateTime? endDate,
            IReadOnlyCollection<string>? kinds,
            string? sortBy,
            string? sortOrder,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = _context.Transactions
                .Include(t => t.Splits)  
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(t => t.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.Date <= endDate.Value);
            }

            var kindsEnum = kinds?.Select(k => Enum.Parse<TransactionKindsEnum>(k, ignoreCase: true)).ToList();

            if (kindsEnum?.Any() == true)
            {
                query = query.Where(t => kindsEnum.Contains(t.Kind));
            }


            if (!string.IsNullOrEmpty(sortBy))
            {
                var isDesc = !string.IsNullOrEmpty(sortOrder) && sortOrder.ToLower() == "desc";

                query = sortBy.ToLower() switch
                {
                    "amount" => isDesc ? query.OrderByDescending(t => t.Amount) : query.OrderBy(t => t.Amount),
                    "kind" => isDesc ? query.OrderByDescending(t => t.Kind) : query.OrderBy(t => t.Kind),
                    "cat-code" => isDesc ? query.OrderByDescending(t => t.CatCode) : query.OrderBy(t => t.CatCode),
                    "direction" => isDesc ? query.OrderByDescending(t => t.Direction) : query.OrderBy(t => t.Direction),
                    "beneficiary-name" => isDesc ? query.OrderByDescending(t => t.BeneficiaryName) : query.OrderBy(t => t.BeneficiaryName),
                    _ => isDesc ? query.OrderByDescending(t => t.Date) : query.OrderBy(t => t.Date) // default sort
                };
            }
            else
            {
                query = query.OrderBy(t => t.Date); 
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var transactions = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(t => t.Category)
                .Include(t => t.Splits)
                .ToListAsync(cancellationToken);

            return (transactions, totalCount);
        }


        public async Task<IEnumerable<Transaction>> GetFilteredAsync(DateTime? startDate, DateTime? endDate, string? catCode, DirectionsEnum? direction, CancellationToken cancellationToken)
        {
            var query = _context.Transactions
                .Include(t => t.Splits)
                .AsQueryable();

            if (!string.IsNullOrEmpty(catCode))
            {
                query = query.Where(t => t.CatCode.StartsWith(catCode));
            }

            if (startDate.HasValue)
            {
                query = query.Where(t => t.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.Date <= endDate.Value);
            }

            if (direction.HasValue)
            {
                query = query.Where(t => t.Direction == direction.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<Transaction> GetByIdWithSplitsAsync(string id)
        {
            return await _context.Transactions
                .Include(t => t.Splits)  
                .FirstOrDefaultAsync(t => t.Id == id)
                ?? throw new RecordNotFoundException("Transaction", id);
        }
    }
}
