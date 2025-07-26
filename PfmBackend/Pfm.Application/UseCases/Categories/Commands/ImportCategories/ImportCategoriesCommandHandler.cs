using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Pfm.Application.Common;
using Pfm.Application.Interfaces;
using Pfm.Domain.Entities;
using Pfm.Domain.Exceptions;
using System.Text;

namespace Pfm.Application.UseCases.Categories.Commands.ImportCategories
{
    public class ImportCategoriesCommandHandler : IRequestHandler<ImportCategoriesCommand, Unit>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICategoriesCsvParser _parser;
        private readonly IValidator<ImportCategoriesDto> _dtoValidator;
        private readonly IValidator<ImportCategoriesCommand> _commandValidator;
        private readonly ILogger<ImportCategoriesCommandHandler> _logger;

        public ImportCategoriesCommandHandler(
            IUnitOfWork uow,
            ICategoriesCsvParser parser,
            IValidator<ImportCategoriesDto> dtoValidator,
            IValidator<ImportCategoriesCommand> commandValidator,
            ILogger<ImportCategoriesCommandHandler> logger)
        {
            _uow = uow;
            _parser = parser;
            _dtoValidator = dtoValidator;
            _commandValidator = commandValidator;
            _logger = logger;
        }

        public async Task<Unit> Handle(ImportCategoriesCommand command, CancellationToken ct)
        {
            // 0. Command validation

            var commandValidationResult = await _commandValidator.ValidateAsync(command, ct);
            if (!commandValidationResult.IsValid)
            {
                throw new ValidationProblemException(commandValidationResult.Errors.Select(e =>
                    new ValidationError("csv", e.ErrorCode, e.ErrorMessage)).ToList());
            }

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(command.CsvContent));

            // 1. Parse CSV with basic validation
            var parseResult = await _parser.ParseAsync(stream);

            // Short-circuit if CSV parsing failed
            if (parseResult.HasErrors)
            {
                _logger.LogWarning("CSV parsing failed with {ErrorCount} errors", parseResult.Errors.Count);
                parseResult.ThrowIfErrors(); // Throws ValidationProblemException
            }

            // 2. Get existing categories once
            var existingCategories = await _uow.Categories.GetAllAsync();
            var validationErrors = new List<ValidationError>();
            var categoriesToProcess = new List<Category>();

            // 3. Process each record with full validation
            foreach (var record in parseResult.ValidRecords)
            {
                // DTO validation
                var dtoValidationResult = await _dtoValidator.ValidateAsync(record, ct);
                if (!dtoValidationResult.IsValid)
                {
                    validationErrors.AddRange(dtoValidationResult.Errors.Select(e =>
                        new ValidationError(record.Code, e.ErrorCode, e.ErrorMessage)));
                    continue;
                }

                
                var category = ProcessCategory(record, existingCategories);
                categoriesToProcess.Add(category);
                
               
            }

            // 4. Handle validation failures
            if (validationErrors.Any())
            {
                _logger.LogWarning("Domain validation failed with {ErrorCount} errors", validationErrors.Count);
                throw new ValidationProblemException(validationErrors);
            }

            // 5. Persist valid categories
            await ProcessCategories(categoriesToProcess);
            await _uow.CompleteAsync();

            _logger.LogInformation("Successfully imported {Count} categories", categoriesToProcess.Count);
            return Unit.Value;
        }

        private Category ProcessCategory(ImportCategoriesDto record, IEnumerable<Category> existingCategories)
        {
            var existing = existingCategories.FirstOrDefault(c => c.Code == record.Code);

            if (existing != null)
            {
                existing.UpdateName(record.Name);
                existing.UpdateParentCode(record.ParentCode);
                return existing;
            }

            // Validate parent exists if specified
            if (!string.IsNullOrEmpty(record.ParentCode) &&
                !existingCategories.Any(c => c.Code == record.ParentCode))
            {
                throw new BusinessRuleException(
                    "invalid-parent",
                    $"Parent category '{record.ParentCode}' does not exist",
                    $"Code: {record.Code}, Parent: {record.ParentCode}");
            }

            return new Category(record.Code, record.Name, record.ParentCode);
        }

        private async Task ProcessCategories(IEnumerable<Category> categories)
        {
            foreach (var category in categories)
            {
                if (category.ParentCode != null)
                {
                    // Ensure the parent exists (double-check)
                    var parentExists = await _uow.Categories.ExistsAsync(category.ParentCode);
                    if (!parentExists)
                    {
                        throw new BusinessRuleException(
                            "invalid-parent",
                            $"Parent category '{category.ParentCode}' not found",
                            $"For category: {category.Code}");
                    }
                }

                if (await _uow.Categories.ExistsAsync(category.Code))
                {
                    await _uow.Categories.UpdateAsync(category);
                }
                else
                {
                    await _uow.Categories.AddAsync(category);
                }
            }
        }
    }

}
