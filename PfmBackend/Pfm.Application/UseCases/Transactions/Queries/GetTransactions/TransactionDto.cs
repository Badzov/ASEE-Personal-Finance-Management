using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Transactions.Queries.GetTransactions
{
    public record TransactionDto(
        string Id,
        string BeneficiaryName,
        DateTime Date,
        string Direction,
        decimal Amount,
        string Description,
        string Currency,
        string Mcc,
        string Kind,
        string CatCode
    );
}
