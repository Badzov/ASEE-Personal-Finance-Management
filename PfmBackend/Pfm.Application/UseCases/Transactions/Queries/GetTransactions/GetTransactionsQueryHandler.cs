using AutoMapper;
using MediatR;
using Pfm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Transactions.Queries.GetTransactions
{
    public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, List<GetTransactionsDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetTransactionsQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<List<GetTransactionsDto>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            var transactions = await _uow.Transactions.GetAllAsync();
            return _mapper.Map<List<GetTransactionsDto>>(transactions);
        }
    }
}
