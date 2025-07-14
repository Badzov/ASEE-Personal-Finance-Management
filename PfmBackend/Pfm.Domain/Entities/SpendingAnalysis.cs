using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Domain.Entities
{
    public class SpendingAnalysis
    {
        public string CatCode { get; }
        public decimal Amount { get; }
        public int Count { get; }

        public SpendingAnalysis(string catCode, decimal amount, int count)
        {
            CatCode = catCode;
            Amount = amount;
            Count = count;
        }
    }
}
