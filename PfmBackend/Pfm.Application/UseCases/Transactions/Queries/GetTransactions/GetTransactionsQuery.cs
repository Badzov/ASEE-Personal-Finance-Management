using MediatR;
using Pfm.Application.Common;
using Pfm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Transactions.Queries.GetTransactions
{
    public record GetTransactionsQuery(TransactionFilters Filters): IRequest<PaginatedResult<TransactionDto>>;
}
