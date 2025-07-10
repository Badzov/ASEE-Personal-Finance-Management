using FluentValidation;
using Pfm.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Transactions.Commands.ImportTransactions
{
    public class ImportTransactionsDtoValidator : AbstractValidator<ImportTransactionsDto>
    {
        public ImportTransactionsDtoValidator()
        {
            RuleFor(x => x.Currency)
                .Length(3).WithMessage("Currency must be 3 characters")
                .Matches(@"^[A-Z]{3}$").WithMessage("Invalid ISO currency code");

            RuleFor(x => x.Direction)
                .Matches(@"^[dc]$").WithMessage("Direction must be 'd' or 'c'");

            RuleFor(x => x.Kind)
                .IsEnumName(typeof(TransactionKind), false)
                .WithMessage("Invalid transaction kind");
        }
    }
}
