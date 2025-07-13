using AutoMapper;
using MediatR;
using Pfm.Application.Common;
using Pfm.Domain.Common;
using Pfm.Application.Exceptions;
using Pfm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                throw new AppException(validationResult.Errors.Select(e => new AppError("filters", e.ErrorCode, e.ErrorMessage)).ToList(), "Invalid filters");
            }

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
    }
}
