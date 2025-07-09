using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.DTOs
{
    public class TransactionDto
    {
        public string Id { get; set; }
        public string BeneficiaryName { get; set; }
        public decimal Amount { get; set; }
    }
}
