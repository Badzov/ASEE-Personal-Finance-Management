using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Transactions.Queries.GetTransactions
{
    public class GetTransactionsDto
    {
        public string Id { get; set; }
        public string BeneficiaryName { get; set; }
        public DateTime Date { get; set; }
        public string Direction { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public string Mcc { get; set; }
        public string Kind { get; set; }
        public string CategoryCode { get; set; }
    }
}
