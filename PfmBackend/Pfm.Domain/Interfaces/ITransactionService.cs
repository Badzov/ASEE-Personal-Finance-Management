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
        Task ImportTransactionsAsync(Stream csvStream);

    }
}
