using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Pfm.Application.Common;
using Pfm.Application.Exceptions;
using Pfm.Domain.Exceptions;
using Pfm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Transactions.Commands.CategorizeTransaction
{
    public class CategorizeTransactionCommandHandler : IRequestHandler<CategorizeTransactionCommand, Unit>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CategorizeTransactionCommandHandler> _logger;
        private readonly IValidator<TransactionCategoryDto> _validator;

        public CategorizeTransactionCommandHandler(
            IUnitOfWork uow,
            ILogger<CategorizeTransactionCommandHandler> logger,
            IValidator<TransactionCategoryDto> validator)
        {
            _uow = uow;
            _logger = logger;
            _validator = validator;
        }

        public async Task<Unit> Handle(CategorizeTransactionCommand request, CancellationToken ct)
        {
            // 1. Apply DTO validation
            var validationResult = await _validator.ValidateAsync(
                new TransactionCategoryDto(request.CategoryCode));
            if (!validationResult.IsValid)
            {
                throw new AppException(
                    validationResult.Errors.Select(e =>
                        new AppError("category", e.ErrorCode, e.ErrorMessage)).ToList(),
                    "Validation failed");
            }

            // 2. Check existence (technically domain validation)
            var transaction = await _uow.Transactions.GetByIdAsync(request.TransactionId)
                ?? throw new DomainException(
                    "transaction-not-found",
                    $"Transaction {request.TransactionId} not found");

            var category = await _uow.Categories.GetByIdAsync(request.CategoryCode)
                ?? throw new DomainException(
                    "category-not-found",
                    $"Category {request.CategoryCode} not found");

            
            transaction.CatCode = request.CategoryCode;
            await _uow.CompleteAsync();

            _logger.LogInformation("Categorized transaction {Id}", request.TransactionId);

            return Unit.Value;
        }
    }
}
