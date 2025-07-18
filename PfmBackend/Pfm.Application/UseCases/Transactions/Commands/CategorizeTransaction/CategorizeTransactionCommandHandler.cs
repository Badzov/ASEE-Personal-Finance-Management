using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Pfm.Application.Common;
using Pfm.Domain.Exceptions;
using Pfm.Domain.Interfaces;

namespace Pfm.Application.UseCases.Transactions.Commands.CategorizeTransaction
{
    public class CategorizeTransactionCommandHandler : IRequestHandler<CategorizeTransactionCommand, Unit>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CategorizeTransactionCommandHandler> _logger;
        private readonly IValidator<CategorizeTransactionCommand> _validator;

        public CategorizeTransactionCommandHandler(
            IUnitOfWork uow,
            ILogger<CategorizeTransactionCommandHandler> logger,
            IValidator<CategorizeTransactionCommand> validator)
        {
            _uow = uow;
            _logger = logger;
            _validator = validator;
        }

        public async Task<Unit> Handle(
            CategorizeTransactionCommand request,
            CancellationToken ct)
        {
            // Validate the command
            var validationResult = await _validator.ValidateAsync(request, ct);

            if (!validationResult.IsValid)
            {
                throw new ValidationProblemException(
                    validationResult.Errors.Select(e =>
                        new ValidationError("command", e.ErrorCode, e.ErrorMessage)));
            }

            // Check existence
            var transaction = await _uow.Transactions.GetByIdAsync(request.TransactionId)
                ?? throw new BusinessRuleException(
                    "transaction-not-found",
                    $"Transaction {request.TransactionId} not found");

            var category = await _uow.Categories.GetByIdAsync(request.CategoryCode)
                ?? throw new BusinessRuleException(
                    "category-not-found",
                    $"Category {request.CategoryCode} not found");

            // Apply changes
            transaction.CatCode = request.CategoryCode;
            await _uow.CompleteAsync();

            _logger.LogInformation(
                "Categorized transaction {TransactionId} with category {CategoryCode}",
                request.TransactionId,
                request.CategoryCode);

            return Unit.Value;
        }
    }
}
