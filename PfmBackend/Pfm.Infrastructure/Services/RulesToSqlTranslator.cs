using Microsoft.Data.SqlClient;
using Npgsql;
using Pfm.Application.Interfaces;
using Pfm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Infrastructure.Services
{
    public sealed class RulesToSqlTranslator : IRulesToSqlTranslator
    {
        private readonly bool _isPostgreSql;

        public RulesToSqlTranslator(bool isPostgreSql)
        {
            _isPostgreSql = isPostgreSql;
        }

        public (string Sql, List<IDbDataParameter> Parameters) Translate(IReadOnlyList<AutoCategorizationRule> rules)
        {
            // Use original casing for mappings (same for both)
            var mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["id"] = "Id",
                ["beneficiary-name"] = "BeneficiaryName",
                ["date"] = "Date",
                ["direction"] = "Direction",
                ["amount"] = "Amount",
                ["description"] = "Description",
                ["currency"] = "Currency",
                ["mcc"] = "Mcc",
                ["kind"] = "Kind",
                ["cat-code"] = "CatCode",
            };

            var parameters = new List<IDbDataParameter>();
            var sqlBuilder = new StringBuilder();

            // Quote table and column names in PostgreSQL
            string tableName = _isPostgreSql ? "\"Transactions\"" : "Transactions";
            string catCodeColumn = _isPostgreSql ? "\"CatCode\"" : "CatCode";

            sqlBuilder.AppendLine($"UPDATE {tableName} SET {catCodeColumn} = CASE");

            for (int i = 0; i < rules.Count; i++)
            {
                var rule = rules[i];

                string sqlCondition = rule.Predicate;

                foreach (var kvp in mappings)
                {
                    // Replace keys with quoted or unquoted column names accordingly
                    var columnName = _isPostgreSql ? $"\"{kvp.Value}\"" : kvp.Value;
                    sqlCondition = sqlCondition.Replace(kvp.Key, columnName);
                }

                string paramName = $"@catCode{i}";
                sqlBuilder.AppendLine($"    WHEN ({sqlCondition}) THEN {paramName}");

                IDbDataParameter param = _isPostgreSql
                    ? new NpgsqlParameter(paramName, rule.CatCode)
                    : new SqlParameter(paramName, rule.CatCode);

                parameters.Add(param);
            }

            sqlBuilder.AppendLine("END");

            // Add WHERE clause with quoting if PostgreSQL
            string whereClause = _isPostgreSql
                ? $"WHERE \"CatCode\" IS NULL"
                : $"WHERE CatCode IS NULL";

            sqlBuilder.AppendLine(whereClause);

            return (sqlBuilder.ToString(), parameters);
        }
    }
}