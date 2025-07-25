using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Infrastructure.Services.RulesProvider
{
    internal sealed record AutoCategorizationRuleDto(
        string Id,
        string Title,
        string CatCode,
        string Predicate
    );
}
