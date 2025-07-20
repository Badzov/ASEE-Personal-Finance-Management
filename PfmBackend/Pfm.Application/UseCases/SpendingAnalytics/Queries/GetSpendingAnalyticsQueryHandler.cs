using AutoMapper;
using FluentValidation;
using MediatR;
using Pfm.Application.Common;
using Pfm.Application.UseCases.Queries;
using Pfm.Domain.Entities;
using Pfm.Domain.Enums;
using Pfm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.SpendingAnalytics.Queries
{
    public class GetSpendingAnalyticsQueryHandler : IRequestHandler<GetSpendingAnalyticsQuery, SpendingsByCategoryDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IValidator<GetSpendingAnalyticsQuery> _validator;

        public GetSpendingAnalyticsQueryHandler(
            IUnitOfWork uow,
            IMapper mapper,
            IValidator<GetSpendingAnalyticsQuery> validator)
        {
            _uow = uow;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<SpendingsByCategoryDto> Handle(GetSpendingAnalyticsQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new ValidationProblemException(validationResult.Errors.Select(e =>
                    new ValidationError(
                        e.PropertyName.ToLower(), 
                        e.ErrorCode,
                        e.ErrorMessage)).ToList());
            }

            DirectionsEnum? directionEnum = request.Direction?.ToLower() switch
            {
                "d" => DirectionsEnum.d,
                "c" => DirectionsEnum.c,
                _ => null
            };

            var transactions = await _uow.Transactions.GetFilteredAsync(
                request.StartDate,
                request.EndDate,
                request.CatCode,
                directionEnum,
                cancellationToken);

            var categorySpendings = new Dictionary<string, (double Amount, int Count)>();

            const string nullCategoryKey = "UNCATEGORIZED";
            categorySpendings[nullCategoryKey] = (0, 0);

            foreach (var transaction in transactions)
            {
                if (transaction.CatCode == "SPLIT" && transaction.Splits?.Count > 0)
                {
                    foreach (var split in transaction.Splits)
                    {
                        if (categorySpendings.ContainsKey(split.CatCode))
                        {
                            categorySpendings[split.CatCode] = (
                                categorySpendings[split.CatCode].Amount + split.Amount,
                                categorySpendings[split.CatCode].Count + 1
                            );
                        }
                        else
                        {
                            categorySpendings[split.CatCode] = (split.Amount, 1);
                        }
                    }
                }
                else
                {
                    var categoryKey = transaction.CatCode ?? nullCategoryKey;
                    if (categorySpendings.ContainsKey(categoryKey))
                    {
                        categorySpendings[categoryKey] = (
                            categorySpendings[categoryKey].Amount + transaction.Amount,
                            categorySpendings[categoryKey].Count + 1
                        );
                    }
                    else
                    {
                        categorySpendings[categoryKey] = (transaction.Amount, 1);
                    }
                }
            }

            var result = categorySpendings
                .Where(kvp => kvp.Value.Count > 0) // Only include categories with transactions
                .Select(kvp => new SpendingInCategoryDto(
                    kvp.Key == nullCategoryKey ? null : kvp.Key,
                    kvp.Value.Amount,
                 kvp.Value.Count))
                .ToList();

            return new SpendingsByCategoryDto(result);
        }
    }
}
