using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Transactions.Commands.AutoCategorizeTransactions
{
    public sealed record AutoCategorizeResultDto(
        int TotalProcessed,     
        int TotalCategorized,  
        double PercentageCategorized 
    );
}
