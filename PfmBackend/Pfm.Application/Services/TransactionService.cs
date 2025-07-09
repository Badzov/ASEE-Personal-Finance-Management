using Pfm.Domain.Entities;
using Pfm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _uow;
        public TransactionService(IUnitOfWork uow) => _uow = uow;


        public async Task<List<Transaction>> GetTransactionsAsync() =>
            (await _uow.Transactions.GetAllAsync()).ToList();

        public Task ImportTransactionsAsync(List<Transaction> transactions)
        {
            throw new NotImplementedException();
        }
        public Task CategorizeTransactionAsync(string id, string catCode)
        {
            throw new NotImplementedException();
        }

        public Task SplitTransactionAsync(string id, List<Split> splits)
        {
            throw new NotImplementedException();
        }

        
    }
}
