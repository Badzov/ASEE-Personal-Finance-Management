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
            // Command validation

            var commandValidationResult = await _commandValidator.ValidateAsync(command, ct);
            if (!commandValidationResult.IsValid)
            {
                throw new ValidationProblemException(commandValidationResult.Errors.Select(e =>
                    new ValidationError("csv", e.ErrorCode, e.ErrorMessage)).ToList());
            }
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(command.CsvContent));

            // Parse CSV with basic validation

            var parseResult = await _parser.ParseAsync(stream);
            if (parseResult.HasErrors) parseResult.ThrowIfErrors();

            // Get existing categories 

            var existingCategories = await _uow.Categories.GetAllAsync();
            var validationErrors = new List<ValidationError>();
            var allCategoriesToProcess = new List<Category>();

            // First pass - Create all Categories without parent validation

            foreach (var record in parseResult.ValidRecords)
            {
                var dtoValidationResult = await _dtoValidator.ValidateAsync(record, ct);
                if (!dtoValidationResult.IsValid)
                {
                    validationErrors.AddRange(dtoValidationResult.Errors.Select(e =>
                        new ValidationError(record.Code, e.ErrorCode, e.ErrorMessage)));
                    continue;
                }

                var category = ProcessCategoryFirstPass(record, existingCategories);
                allCategoriesToProcess.Add(category);
            }

            if (validationErrors.Any())
            {
                throw new ValidationProblemException(validationErrors);
            }

            // Second pass - Validate parent relationships

            foreach (var category in allCategoriesToProcess)
            {
                if (category.ParentCode != null)
                {
                    var parentExists = allCategoriesToProcess.Any(c => c.Code == category.ParentCode) ||
                                     existingCategories.Any(c => c.Code == category.ParentCode);

                    if (!parentExists)
                    {
                        throw new BusinessRuleException(
                            "invalid-parent",
                            $"Parent category '{category.ParentCode}' does not exist in file or database",
                            $"For category: {category.Code}");
                    }
                }
            }

            // Process all categories
            await ProcessCategories(allCategoriesToProcess);
            await _uow.CompleteAsync();

            _logger.LogInformation("Successfully imported {Count} categories", allCategoriesToProcess.Count);
            return Unit.Value;
        }

        private Category ProcessCategoryFirstPass(ImportCategoriesDto record, IEnumerable<Category> existingCategories)
        {
            var existing = existingCategories.FirstOrDefault(c => c.Code == record.Code);

            if (existing != null)
            {
                existing.UpdateName(record.Name);
                existing.UpdateParentCode(record.ParentCode);
                return existing;
            }

            // Skip parent validation in first pass
            return new Category(record.Code, record.Name, record.ParentCode);
        }

        private async Task ProcessCategories(IEnumerable<Category> categories)
        {
            var updates = new List<Category>();
            var inserts = new List<Category>();

            foreach (var category in categories)
            {
                if (await _uow.Categories.ExistsAsync(category.Code))
                {
                    updates.Add(category);
                }
                else
                {
                    inserts.Add(category);
                }
            }

            if (inserts.Any())
            {
                await _uow.Categories.AddRangeAsync(inserts);
            }

            foreach (var category in updates)
            {
                await _uow.Categories.UpdateAsync(category);
            }
        }
    }

}
