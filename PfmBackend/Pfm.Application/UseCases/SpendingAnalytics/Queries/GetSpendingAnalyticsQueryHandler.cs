using AutoMapper;
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

        public GetSpendingAnalyticsQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<SpendingsByCategoryDto> Handle(GetSpendingAnalyticsQuery request, CancellationToken cancellationToken)
        {
            DirectionsEnum? directionEnum = null;
            if (!string.IsNullOrEmpty(request.Direction))
            {
                directionEnum = request.Direction.ToLower() switch
                {
                    "d" => DirectionsEnum.Debit,
                    "c" => DirectionsEnum.Credit,
                    _ => throw new ValidationProblemException(
                        new List<ValidationError>
                        {
                            new("direction", "invalid-direction", "Direction must be 'd' or 'c'")
                        })
                };
            }

            var transactions = await _uow.Transactions.GetFilteredAsync(
                request.StartDate,
                request.EndDate,
                request.CatCode,
                directionEnum,
                cancellationToken);

            var groups = transactions
                .GroupBy(t => t.CatCode)
                .Select(g => new SpendingInCategoryDto(
                    g.Key,
                    g.Sum(t => t.Amount),
                    g.Count()))
                .ToList();

            return new SpendingsByCategoryDto(groups);
        }
    }
}
