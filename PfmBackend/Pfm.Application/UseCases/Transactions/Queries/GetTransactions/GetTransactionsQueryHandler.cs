using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Pfm.Application.Common;
using Pfm.Domain.Common;
using Pfm.Domain.Enums;
using Pfm.Domain.Exceptions;
using Pfm.Application.Interfaces;
using System.Text.RegularExpressions;

namespace Pfm.Application.UseCases.Transactions.Queries.GetTransactions
{
    public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, PagedList<TransactionDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IValidator<TransactionFilters> _filtersValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetTransactionsQueryHandler(IUnitOfWork uow, IMapper mapper, IValidator<TransactionFilters> filtersValidator, IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _mapper = mapper;
            _filtersValidator = filtersValidator;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedList<TransactionDto>> Handle(GetTransactionsQuery query, CancellationToken cancellationToken)
        {
            var validationResult = await _filtersValidator.ValidateAsync(query.Filters);

            var fluentErrors = validationResult.Errors
                .Select(e => new ValidationError(
                    Regex.Replace(e.PropertyName, "(?<!^)([A-Z])", "-$1").ToLower(),
                    e.ErrorCode,
                    e.ErrorMessage))
                .ToList();

            var modelErrors = _httpContextAccessor.HttpContext.Items["ModelValidationErrors"] as List<ValidationError> ?? new();

            var allErrors = modelErrors.Concat(fluentErrors).ToList();

            if (allErrors.Any())
                throw new ValidationProblemException(allErrors);

            try
            {
                var (transactions, totalCount) = await _uow.Transactions.GetFilteredAsync(
                    query.Filters.StartDate,
                    query.Filters.EndDate,
                    query.Filters.Kinds,
                    query.Filters.SortBy,
                    query.Filters.SortOrder,
                    query.Filters.PageNumber,
                    query.Filters.PageSize,
                    cancellationToken
                );

                var totalPages = (int)Math.Ceiling(totalCount / (double)query.Filters.PageSize);

                return new PagedList<TransactionDto>(
                    totalCount,
                    query.Filters.PageSize,
                    query.Filters.PageNumber,
                    totalPages,
                    GetSortOrderEnum(query.Filters.SortOrder),
                    query.Filters.SortBy ?? "date",
                    _mapper.Map<List<TransactionDto>>(transactions)
                );
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException(
                    "transaction-retrieval-failed",
                    "Failed to retrieve transactions",
                    ex.Message);
            }
        }

        private SortOrderEnum GetSortOrderEnum(string? sortOrder)
        {
            if (string.IsNullOrEmpty(sortOrder)) return SortOrderEnum.asc;
            return sortOrder.ToLower() == "desc" ? SortOrderEnum.desc : SortOrderEnum.asc;
        }
    }
}
