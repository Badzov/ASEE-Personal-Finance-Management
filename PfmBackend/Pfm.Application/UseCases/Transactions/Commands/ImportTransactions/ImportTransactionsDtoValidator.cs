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
                .NotEmpty().WithErrorCode("id-required").WithMessage("Transaction ID required")
                .MaximumLength(8).WithErrorCode("id-length").WithMessage("Transaction ID must be max 8 characters");

            RuleFor(x => x.Date)
                .NotEmpty().WithErrorCode("date-required").WithMessage("Date required")
                .LessThanOrEqualTo(DateTime.Today).WithErrorCode("future-date").WithMessage("Date cannot be future");

            RuleFor(x => x.Direction)
                .Must(x => Enum.TryParse<DirectionsEnum>(x, out _))
                .WithErrorCode("invalid-direction")
                .WithMessage("Direction must be 'd' (debit) or 'c' (credit)");

            RuleFor(x => x.Amount)
                .NotEmpty().WithErrorCode("amount-required").WithMessage("Amount required");

            RuleFor(x => x.Currency)
                .NotEmpty().WithErrorCode("currency-required").WithMessage("Currency required")
                .Length(3).WithErrorCode("currency-length").WithMessage("Must be 3 characters")
                .Matches(@"^[A-Z]{3}$").WithErrorCode("currecny-format").WithMessage("Currency must be ISO currency code");

            // Optional fields
            RuleFor(x => x.BeneficiaryName)
                .MaximumLength(50).WithErrorCode("name-length").WithMessage("Beneficiary Name can have max 50 characters");

            RuleFor(x => x.Description)
                .MaximumLength(100).WithErrorCode("desc-length").WithMessage("Description can have max 100 characters");

            RuleFor(x => x.Mcc)
                .Must(x => !x.HasValue || Enum.IsDefined(typeof(MccCodeEnum), x.Value))
                .WithErrorCode("invalid-mcc")
                .WithMessage("Invalid MCC code");

            RuleFor(x => x.Kind)
                .Must(x => Enum.TryParse<TransactionKindsEnum>(x, ignoreCase: true, out _))
                .WithErrorCode("invalid-kind")
                .WithMessage("Invalid transaction kind");
        }
    }
}
