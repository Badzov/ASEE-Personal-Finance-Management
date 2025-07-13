using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Pfm.Application.Common;
using Pfm.Application.Exceptions;
using Pfm.Application.Interfaces;
using Pfm.Application.UseCases.Transactions.Commands.ImportTransactions;
using Pfm.Domain.Entities;
using Pfm.Domain.Exceptions;
using Pfm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Categories.Commands.ImportCategories
{
    public class ImportCategoriesCommandHandler : IRequestHandler<ImportCategoriesCommand, Unit>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICategoriesCsvParser _parser;
        private readonly IMapper _mapper;

        public ImportCategoriesCommandHandler(
            IUnitOfWork uow,
            ICategoriesCsvParser parser,
            IMapper mapper)
        {
            _uow = uow;
            _parser = parser;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(ImportCategoriesCommand command, CancellationToken ct)
        {
            // 1. Parse CSV with basic validation
            var parseResult = _parser.Parse(command.CsvStream);

            // 2. Apply DTO validation
            var validator = new ImportCategoriesDtoValidator();
            foreach (var record in parseResult.ValidRecords.ToList())
            {
                var validationResult = await validator.ValidateAsync(record);
                if (!validationResult.IsValid)
                {
                    parseResult.ValidRecords.Remove(record);
                    parseResult.Errors.AddRange(validationResult.Errors.Select(e =>
                        new RecordError(record.Code, e.ErrorCode, e.ErrorMessage)));
                }
            }

            // Short-circuit if any CSV or DTO errors exist
            if (parseResult.HasErrors)
            {
                throw new AppException(
                    parseResult.Errors.Select(e =>
                        new AppError(e.RecordId, e.ErrorCode, e.Message)).ToList(),
                    "CSV/DTO validation failed");
            }

            // 3. Apply Domain (business) validation
            var existingCategories = await _uow.Categories.GetAllAsync();
            var domainErrors = new List<AppError>();
            var validCategories = new List<Category>();

            foreach (var record in parseResult.ValidRecords)
            {
                try
                {
                    var existing = existingCategories.FirstOrDefault(c => c.Code == record.Code);
                    if (existing != null)
                    {
                        existing.UpdateName(record.Name);
                        validCategories.Add(existing);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(record.ParentCode) &&
                            !existingCategories.Any(c => c.Code == record.ParentCode))
                        {
                            throw new DomainException(
                                "invalid-parent",
                                $"Parent code '{record.ParentCode}' does not exist");
                        }

                        var newCategory = new Category(record.Code, record.Name, record.ParentCode);
                        validCategories.Add(newCategory);
                    }
                }
                catch (DomainException ex)
                {
                    domainErrors.Add(new AppError(record.Code, ex.ErrorCode, ex.Message));
                }
            }

            if (domainErrors.Any())
            {
                throw new DomainValidationException(domainErrors);
            }

            // Persist changes
            foreach (var category in validCategories.Where(c => c.Code != null))
            {
                if (existingCategories.Any(c => c.Code == category.Code))
                {
                    await _uow.Categories.UpdateAsync(category);
                }
                else
                {
                    await _uow.Categories.AddAsync(category);
                }
            }

            await _uow.CompleteAsync();
            return Unit.Value;
        }
    }

}
