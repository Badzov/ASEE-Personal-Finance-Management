﻿using System;
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
            RuleFor(x => x.CsvContent)
                .NotEmpty().WithErrorCode("required").WithMessage("CSV content required")
                .Must(BeValidCsv).WithErrorCode("invalid-csv").WithMessage("Invalid CSV format");
        }

        private bool BeValidCsv(string content)
        {
            return !string.IsNullOrWhiteSpace(content) &&
                   content.Contains('\n');
        }
    }
}
