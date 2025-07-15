using AutoMapper;
using MediatR;
using Pfm.Application.Common;
using Pfm.Domain.Common;
using Pfm.Domain.Exceptions;
using Pfm.Domain.Interfaces;

namespace Pfm.Application.UseCases.Transactions.Queries.GetTransactions
{
    public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, PaginatedResult<TransactionDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetTransactionsQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<TransactionDto>> Handle(GetTransactionsQuery query, CancellationToken cancellationToken)
        {
            var validator = new TransactionFiltersValidator();
            var validationResult = await validator.ValidateAsync(query.Filters);

            if (!validationResult.IsValid)
            {
                throw new ValidationProblemException(validationResult.Errors.Select(e => new ValidationError("filters", e.ErrorCode, e.ErrorMessage)).ToList());
            }

            try
            {
                var (transactions, totalCount) = await _uow.Transactions.GetFilteredAsync(
                    query.Filters.StartDate,
                    query.Filters.EndDate,
                    query.Filters.Kinds,
                    query.Filters.PageNumber,
                    query.Filters.PageSize,
                    cancellationToken
                );

                return new PaginatedResult<TransactionDto>(
                    _mapper.Map<List<TransactionDto>>(transactions),
                    totalCount,
                    query.Filters.PageNumber,
                    query.Filters.PageSize
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
    }
}
