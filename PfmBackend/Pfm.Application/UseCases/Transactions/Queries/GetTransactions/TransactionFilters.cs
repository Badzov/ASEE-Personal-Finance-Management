using Pfm.Domain.Enums;

namespace Pfm.Application.UseCases.Transactions.Queries.GetTransactions
{
    public record TransactionFilters(
        DateTime? StartDate,
        DateTime? EndDate,
        IReadOnlyCollection<TransactionKind>? Kinds,
        int PageNumber = 1,
        int PageSize = 10
    )
    {
        public bool HasDateFilter => StartDate != null || EndDate != null;
        public bool HasKindFilter => Kinds?.Any() == true;

        public static TransactionFilters Create(
            DateTime? startDate,
            DateTime? endDate,
            IEnumerable<string>? kindStrings,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var kinds = kindStrings?
                .Select(k => Enum.Parse<TransactionKind>(k, ignoreCase: true))
                .Distinct()
                .ToList();

            return new TransactionFilters(
                startDate,
                endDate,
                kinds,
                pageNumber,
                pageSize
            );
        }
    };
}
