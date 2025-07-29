using Microsoft.Data.SqlClient;
using Pfm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.Interfaces
{
    public interface IRulesToSqlTranslator
    {
        public (string Sql, List<IDbDataParameter> Parameters) Translate(IReadOnlyList<AutoCategorizationRule> rules);
    }
}
