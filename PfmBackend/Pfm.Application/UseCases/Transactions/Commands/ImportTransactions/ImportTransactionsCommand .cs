using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Transactions.Commands.ImportTransactions
{
    public class ImportTransactionsCommand : IRequest<Unit>
    {
        public Stream CsvStream { get; }

        public ImportTransactionsCommand(Stream csvStream)
        {
            CsvStream = csvStream;
        }
    }
}
