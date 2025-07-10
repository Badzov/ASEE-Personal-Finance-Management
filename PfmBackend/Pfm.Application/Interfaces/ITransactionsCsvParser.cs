using Pfm.Application.Common;
using Pfm.Application.UseCases.Transactions.Commands.ImportTransactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.Interfaces
{
    public interface ITransactionCsvParser
    {
        Task<ImportResult<ImportTransactionsDto>> ParseAsync(Stream csvStream);
    }
}
