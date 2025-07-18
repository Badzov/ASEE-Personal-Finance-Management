using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Categories.Commands.ImportCategories
{
    public class ImportCategoriesCommandValidator : AbstractValidator<ImportCategoriesCommand>
    {
        public ImportCategoriesCommandValidator()
        {
            RuleFor(x => x.CsvContent)
                .NotEmpty().WithErrorCode("required").WithMessage("CSV content is required")
                .Must(BeValidCsv).WithErrorCode("invalid-csv").WithMessage("Invalid CSV format");
        }

        private bool BeValidCsv(string content)
        {
            return !string.IsNullOrWhiteSpace(content) &&
                   content.Contains('\n'); 
        }
    }
}
