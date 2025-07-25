using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Transactions.Commands.AutoCategorizeTransactions
{
    public record AutoCategorizeTransactionsCommand : IRequest<AutoCategorizeResultDto>;
}
