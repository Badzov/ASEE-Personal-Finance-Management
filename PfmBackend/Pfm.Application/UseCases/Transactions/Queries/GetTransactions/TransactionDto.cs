using Pfm.Domain.Entities;
using Pfm.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Transactions.Queries.GetTransactions
{
    public record TransactionDto(
        string Id,
        string? BeneficiaryName,
        DateTime Date,
        DirectionsEnum Direction,
        double Amount,
        string? Description,
        string Currency,
        MccCodeEnum? Mcc,
        TransactionKindsEnum Kind,
        string? CatCode,
        ICollection<SingleCategorySplitDto>? Splits
    );
}
