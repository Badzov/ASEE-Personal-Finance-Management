using Pfm.Domain.Enums;

namespace Pfm.Application.UseCases.Transactions.Queries.GetTransactions
{
    public record TransactionFilters(
        DateTime? StartDate,
        DateTime? EndDate,
        IReadOnlyCollection<string>? Kinds,
        string? SortBy,
        string? SortOrder,
        int PageNumber = 1,
        int PageSize = 10
    )
    {
        public bool HasDateFilter => StartDate != null || EndDate != null;
        public bool HasKindFilter => Kinds?.Any() == true;

        public static TransactionFilters Create(
            DateTime? startDate,
            DateTime? endDate,
            IReadOnlyCollection<string>? kindStrings,
            string? sortBy,
            string? sortOrder,
            int page = 1,
            int pageSize = 10)
        {

            return new TransactionFilters(
                startDate,
                endDate,
                kindStrings,
                sortBy,
                sortOrder,
                page,
                pageSize
            );
        }
    };
}
