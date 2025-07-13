using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Transactions.Commands.CategorizeTransaction
{
    public record CategorizeTransactionCommand(string TransactionId, string CategoryCode) : IRequest<Unit>;
}
