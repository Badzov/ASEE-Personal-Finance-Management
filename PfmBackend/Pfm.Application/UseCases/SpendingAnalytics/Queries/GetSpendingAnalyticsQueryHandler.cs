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
            Console.WriteLine(">>>>>>>>>>>" + request.StartDate);

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
