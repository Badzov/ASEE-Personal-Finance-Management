using Pfm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Transaction> Transactions { get; }
        IRepository<Category> Categories { get; }
        IRepository<Split> Splits { get; }
        Task<int> CompleteAsync();
    }
}
