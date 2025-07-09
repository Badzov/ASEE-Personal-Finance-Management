using Pfm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Domain.Interfaces
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetTransactionsAsync();
        Task ImportTransactionsAsync(List<Transaction> transactions);
        Task CategorizeTransactionAsync(string id, string catCode);
        Task SplitTransactionAsync(string id, List<Split> splits);
    }
}
