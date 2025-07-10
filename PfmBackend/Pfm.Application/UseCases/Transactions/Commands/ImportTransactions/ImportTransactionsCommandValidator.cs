using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pfm.Application.UseCases.Transactions.Commands.ImportTransactions
{
    public class ImportTransactionsCommandValidator : AbstractValidator<ImportTransactionsCommand>
    {
        public ImportTransactionsCommandValidator()
        {
            RuleFor(x => x.CsvStream)
                .NotNull().WithMessage("CSV stream is required")
                .Must(stream => stream?.Length > 0).WithMessage("CSV stream cannot be empty");
        }
    }
}
