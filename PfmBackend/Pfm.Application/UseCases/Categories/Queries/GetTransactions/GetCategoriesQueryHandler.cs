using AutoMapper;
using FluentValidation;
using MediatR;
using Pfm.Application.Common;
using Pfm.Application.Interfaces;
using Pfm.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Categories.Queries.GetTransactions
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IValidator<GetCategoriesQuery> _validator;

        public GetCategoriesQueryHandler(IUnitOfWork uow, IMapper mapper, IValidator<GetCategoriesQuery> validator)
        {
            _uow = uow;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<List<CategoryDto>> Handle(GetCategoriesQuery query, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationProblemException(validationResult.Errors
                    .Select(e => new ValidationError(
                        Regex.Replace(e.PropertyName, "(?<!^)([A-Z])", "-$1").ToLower(),
                        e.ErrorCode,
                        e.ErrorMessage))
                    .ToList());
            }

            IEnumerable<Category> categories = string.IsNullOrEmpty(query.ParentCode)
                ? await _uow.Categories.GetAllAsync()
                : await _uow.Categories.GetByParentCodeAsync(query.ParentCode);

            return _mapper.Map<List<CategoryDto>>(categories);
        }
    }
}
