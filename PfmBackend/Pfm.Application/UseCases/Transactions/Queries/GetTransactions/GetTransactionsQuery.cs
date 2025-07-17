using MediatR;
using Pfm.Domain.Common;

namespace Pfm.Application.UseCases.Transactions.Queries.GetTransactions
{
    public record GetTransactionsQuery(TransactionFilters Filters): IRequest<PagedList<TransactionDto>>;
}
