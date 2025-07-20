using FluentValidation;
using MediatR;
using Pfm.Application.Common;
using Pfm.Domain.Entities;
using Pfm.Domain.Exceptions;
using Pfm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Transactions.Commands.SplitTransaction
{
    public class SplitTransactionCommandHandler : IRequestHandler<SplitTransactionCommand, Unit>
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<SplitTransactionCommand> _commandValidator;

        public SplitTransactionCommandHandler(IUnitOfWork uow, IValidator<SplitTransactionCommand> commandValidator)
        {
            _uow = uow;
            _commandValidator = commandValidator;
        }

        public async Task<Unit> Handle(SplitTransactionCommand request, CancellationToken ct)
        {
            // Command validation
            var validationResult = await _commandValidator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                throw new ValidationProblemException(
                    validationResult.Errors.Select(e =>
                        new ValidationError(
                            e.PropertyName,
                            e.ErrorCode,
                            e.ErrorMessage
                        )
                    )
                );
            }

            // Get transaction with existing splits (throws RecordNotFound if non existant)
            var transaction = await _uow.Transactions.GetByIdWithSplitsAsync(request.TransactionId);


            // Verify all categories exist
            var categoryCodes = request.Splits.Select(s => s.CatCode).Distinct();
            var existingCategories = await _uow.Categories.GetByCodesAsync(categoryCodes);

            var missingCategories = categoryCodes
                .Except(existingCategories.Select(c => c.Code))
                .ToList();

            if (missingCategories.Any())
            {
                throw new BusinessRuleException("category-not-found",
                    $"Categories not found: {string.Join(", ", missingCategories)}");
            }

            //Mark the transaction category code as SPLIT
            transaction.CatCode = "SPLIT";

            // Map DTOs to domain models
            var splits = request.Splits.Select(s => new SingleCategorySplit
            {
                TransactionId = request.TransactionId,
                CatCode = s.CatCode,
                Amount = s.Amount
            }).ToList();

            // Validate at domain level
            transaction.ValidateSplit(splits);

            // Replace existing splits
            await _uow.Splits.DeleteByTransactionIdAsync(request.TransactionId);
            await _uow.Splits.AddRangeAsync(splits);

            await _uow.CompleteAsync();

            return Unit.Value;
        }
    }
}
