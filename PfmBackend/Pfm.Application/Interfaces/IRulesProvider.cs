using Pfm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.Interfaces
{
    public interface IRulesProvider
    {
        IReadOnlyList<AutoCategorizationRule> GetRules();
    };

}
