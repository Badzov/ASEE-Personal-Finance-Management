using Microsoft.Data.SqlClient;
using Pfm.Application.Interfaces;
using Pfm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Infrastructure.Services
{
    public sealed class RulesToSqlTranslator : IRulesToSqlTranslator
    {
        public (string Sql, List<SqlParameter> Parameters) Translate(IReadOnlyList<AutoCategorizationRule> rules)
        {
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

            var parameters = new List<SqlParameter>();
            var sqlBuilder = new StringBuilder();

            // Base SQL
            sqlBuilder.AppendLine("UPDATE Transactions SET CatCode = CASE");

            // Add WHEN clauses
            for (int i = 0; i < rules.Count; i++)
            {
                var rule = rules[i];

                string sqlCondition = rule.Predicate;

                foreach (var kvp in mappings)
                {
                    sqlCondition = sqlCondition.Replace(kvp.Key, kvp.Value);
                }

                sqlBuilder.AppendLine($"    WHEN ({sqlCondition}) THEN @catCode{i}");
                parameters.Add(new SqlParameter($"@catCode{i}", rule.CatCode));
            }

            // Finalize
            sqlBuilder.AppendLine("END");
            sqlBuilder.AppendLine("WHERE CatCode IS NULL");

            return (sqlBuilder.ToString(), parameters);
        }
    }
}
