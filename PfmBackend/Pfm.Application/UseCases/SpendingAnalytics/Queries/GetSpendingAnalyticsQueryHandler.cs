using AutoMapper;
using MediatR;
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
    public class GetSpendingAnalyticsQueryHandler
    : IRequestHandler<GetSpendingAnalyticsQuery, IEnumerable<SpendingAnalysisDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetSpendingAnalyticsQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SpendingAnalysisDto>> Handle(
            GetSpendingAnalyticsQuery request,
            CancellationToken cancellationToken)
        {
            TransactionDirection? direction = request.Direction switch
            {
                "d" => TransactionDirection.Debit,
                "c" => TransactionDirection.Credit,
                _ => null
            };

            var analytics = await _uow.SpendingAnalytics.GetFilteredAsync(
                request.CatCode,
                request.StartDate,
                request.EndDate,
                direction);

            return _mapper.Map<IEnumerable<SpendingAnalysisDto>>(analytics);
        }
    }
}
