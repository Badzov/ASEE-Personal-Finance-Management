using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Transactions.Commands.CategorizeTransaction
{
    public record TransactionCategoryDto(
        string CategoryCode
    );
}
