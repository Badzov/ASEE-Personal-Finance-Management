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
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Transaction ID is required")
                .Length(1, 8).WithMessage("Transaction ID must be up to 8 characters");

            RuleFor(x => x.BeneficiaryName)
                .MaximumLength(50).WithMessage("Beneficiary name must be up to 50 characters");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required")
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Date cannot be in the future");

            RuleFor(x => x.Direction)
                .IsInEnum().WithMessage("Invalid transaction direction");

            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("Amount is required");

            RuleFor(x => x.Description)
                .MaximumLength(100).WithMessage("Description must be up to 100 characters");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required")
                .Length(3).WithMessage("Currency must be 3 characters")
                .Matches(@"^[A-Z]{3}$").WithMessage("Invalid ISO currency code");

            RuleFor(x => x.Mcc)
                .IsInEnum().WithMessage("Invalid MCC code") 
                .When(x => x.Mcc.HasValue);

            RuleFor(x => x.Kind)
                .IsInEnum().WithMessage("Invalid transaction kind");
        }
    }
}
