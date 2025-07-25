using Pfm.Application.Common;
using Pfm.Application.UseCases.Transactions.Commands.ImportTransactions;

namespace Pfm.Application.Interfaces
{
    public interface ITransactionsCsvParser
    {
        Task<ImportResult<ImportTransactionsDto>> ParseAsync(Stream stream);
    }

    

}
