using AutoMapper;
using FluentValidation;
using MediatR;
using Pfm.Application.Common;
using Pfm.Application.UseCases.Transactions.Commands.ImportTransactions;
using Pfm.Domain.Common;
using Pfm.Domain.Enums;
using Pfm.Domain.Exceptions;
using Pfm.Domain.Interfaces;

namespace Pfm.Application.UseCases.Transactions.Queries.GetTransactions
{
    public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, PagedList<TransactionDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IValidator<TransactionFilters> _filtersValidator;

        public GetTransactionsQueryHandler(IUnitOfWork uow, IMapper mapper, IValidator<TransactionFilters> filtersValidator)
        {
            _uow = uow;
            _mapper = mapper;
            _filtersValidator = filtersValidator;
        }

        public async Task<PagedList<TransactionDto>> Handle(GetTransactionsQuery query, CancellationToken cancellationToken)
        {
            var validationResult = await _filtersValidator.ValidateAsync(query.Filters);

            if (!validationResult.IsValid)
            {
                throw new ValidationProblemException(validationResult.Errors.Select(e =>
                    new ValidationError("filters", e.ErrorCode, e.ErrorMessage)).ToList());
            }

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
