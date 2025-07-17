using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.Categories.Commands.ImportCategories
{
    public record ImportCategoriesCommand(string CsvContent) : IRequest<Unit>;
}
