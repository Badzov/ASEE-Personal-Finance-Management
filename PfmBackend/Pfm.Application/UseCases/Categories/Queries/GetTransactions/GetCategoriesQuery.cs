using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Categories.Queries.GetTransactions
{
    public record GetCategoriesQuery(string? ParentCode) : IRequest<List<CategoryDto>>;
}
