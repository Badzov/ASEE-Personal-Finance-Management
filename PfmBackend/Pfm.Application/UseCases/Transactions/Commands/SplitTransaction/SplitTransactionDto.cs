using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Transactions.Commands.SplitTransaction
{
    public class SplitTransactionDto
    {
        public string CatCode { get; set; }
        public double Amount { get; set; }
    }
}
