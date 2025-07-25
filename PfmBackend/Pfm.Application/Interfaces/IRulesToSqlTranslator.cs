using Microsoft.Data.SqlClient;
using Pfm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.Interfaces
{
    public interface IRulesToSqlTranslator
    {
        public (string Sql, List<SqlParameter> Parameters) Translate(IReadOnlyList<AutoCategorizationRule> rules);
    }
}
