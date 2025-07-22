using AutoMapper;
using FluentValidation;
using MediatR;
using Pfm.Application.Common;
using Pfm.Application.UseCases.Queries;
using Pfm.Domain.Entities;
using Pfm.Domain.Enums;
using Pfm.Domain.Interfaces;
using Pfm.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.SpendingAnalytics.Queries
{
    public sealed class GetSpendingAnalyticsQueryHandler
    : IRequestHandler<GetSpendingAnalyticsQuery, SpendingsByCategoryDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly CategoryHierarchyService _categoryService;
        private readonly IValidator<GetSpendingAnalyticsQuery> _validator;

        public GetSpendingAnalyticsQueryHandler(IUnitOfWork uow, CategoryHierarchyService categoryService, IValidator<GetSpendingAnalyticsQuery> validator)
        {
            _uow = uow;
            _categoryService = categoryService;
            _validator = validator;
        }

        public async Task<SpendingsByCategoryDto> Handle(GetSpendingAnalyticsQuery request,CancellationToken ct)
        {
            await ValidateRequest(request, ct);

            var (transactions, categories) = await GetDataAsync(request, ct);

            var results = ProcessTransactions(transactions, request.CatCode);

            return BuildResponse(results, request.CatCode, categories);
        }

        private async Task ValidateRequest(GetSpendingAnalyticsQuery request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request, ct);

            if (!validationResult.IsValid)
            {
                throw new ValidationProblemException(validationResult.Errors.Select(e =>
                    new ValidationError(
                        e.PropertyName.ToLower(),
                        e.ErrorCode,
                        e.ErrorMessage)).ToList());
            }
        }

        private async Task<(IEnumerable<Transaction> transactions, IEnumerable<Category> categories)> GetDataAsync(GetSpendingAnalyticsQuery request, CancellationToken ct)
        {
            var directionEnum = ParseDirection(request.Direction);
            var transactions = await _uow.Transactions.GetFilteredAsync(request.StartDate, request.EndDate, catCode: null, directionEnum, ct);
            var categories = await _uow.Categories.GetAllAsync();
            return (transactions, categories);
        }

        private static DirectionsEnum? ParseDirection(string? direction) =>
            direction?.ToLower() switch
            {
                "d" => DirectionsEnum.d,
                "c" => DirectionsEnum.c,
                _ => null
            };

        private Dictionary<string, SpendingData> ProcessTransactions(IEnumerable<Transaction> transactions, string? filterCategoryCode)
        {
            const string uncategorizedKey = "UNCATEGORIZED";
            var results = new Dictionary<string, SpendingData>();

            foreach (var txn in transactions)
            {
                if (txn.Splits?.Count > 0)
                {
                    ProcessSplits(txn, filterCategoryCode, results);
                }
                else
                {
                    ProcessTransaction(txn, filterCategoryCode, results, uncategorizedKey);
                }
            }

            return results;
        }

        private void ProcessSplits(Transaction txn, string? filterCategoryCode, Dictionary<string, SpendingData> results)
        {
            foreach (var split in txn.Splits)
            {
                var reportingKey = filterCategoryCode == null
                    ? _categoryService.GetTopLevelParentCode(split.CatCode) ?? "UNCATEGORIZED"
                    : split.CatCode;

                AggregateSpending(results, reportingKey, split.Amount);
            }
        }

        private void ProcessTransaction(Transaction txn, string? filterCategoryCode,Dictionary<string, SpendingData> results, string uncategorizedKey)
        {
            var reportingKey = filterCategoryCode == null
                ? _categoryService.GetTopLevelParentCode(txn.CatCode) ?? uncategorizedKey
                : txn.CatCode ?? uncategorizedKey;

            AggregateSpending(results, reportingKey, txn.Amount);
        }

        private static SpendingsByCategoryDto BuildResponse(Dictionary<string, SpendingData> results, string? filterCategoryCode, IEnumerable<Category> categories)
        {
            var filteredGroups = results
                .Where(kvp => ShouldIncludeCategory(kvp.Key, filterCategoryCode, categories))
                .Select(kvp => new SpendingInCategoryDto(
                    kvp.Key == "UNCATEGORIZED" ? "UNCATEGORIZED" : kvp.Key,
                    kvp.Value.Amount,
                    kvp.Value.Count))
                .ToList();

            return new SpendingsByCategoryDto(filteredGroups);
        }

        private static bool ShouldIncludeCategory(string categoryKey, string? filterCategoryCode, IEnumerable<Category> categories)
        {
            if (filterCategoryCode == null)
            {
                return categoryKey == "UNCATEGORIZED" || categories.Any(c => c.Code == categoryKey && c.ParentCode == null);
            }

            return categoryKey == filterCategoryCode ||  categories.Any(c => c.Code == categoryKey && c.ParentCode == filterCategoryCode);
        }

        private static void AggregateSpending(Dictionary<string, SpendingData> results, string categoryKey, double amount)
        {
            if (results.TryGetValue(categoryKey, out var existing))
            {
                results[categoryKey] = existing with
                {
                    Amount = existing.Amount + amount,
                    Count = existing.Count + 1
                };
            }
            else
            {
                results[categoryKey] = new SpendingData(amount, 1);
            }
        }

        private record SpendingData(double Amount, int Count);
    }
}
