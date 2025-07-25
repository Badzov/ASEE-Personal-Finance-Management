using Pfm.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Domain.Entities
{
    public sealed record AutoCategorizationRule(string Id, string Title, string CatCode, string Predicate)
    {
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(CatCode))
                throw new BusinessRuleException("invalid-rule", "CatCode is required");

            if (string.IsNullOrWhiteSpace(Predicate))
                throw new BusinessRuleException("invalid-rule", "Predicate is required");

            if (!IsValidPredicate(Predicate))
                throw new BusinessRuleException("invalid-predicate",
                    $"Predicate contains invalid characters or patterns: {Predicate}");
        }

        private static bool IsValidPredicate(string predicate)
        {
            // 1. Basic SQL injection patterns
            var forbiddenPatterns = new[]
            {
                ";", "--", "/*", "*/",
                "DROP ", "DELETE ", "UPDATE ",
                "INSERT ", "EXEC ", "TRUNCATE "
            };

            if (forbiddenPatterns.Any(p => predicate.Contains(p, StringComparison.OrdinalIgnoreCase)))
                return false;

            // 2. Only allow specific operators
            var allowedOperators = new[] { "=", "!=", "<", ">", "<=", ">=", "LIKE", "IN", "IS" };
            if (!allowedOperators.Any(op => predicate.Contains(op, StringComparison.OrdinalIgnoreCase)))
                return false;

            // 3. Field whitelist
            var allowedFields = new[] { "beneficiary-name", "mcc", "amount", "date", "direction", "description", "kind" };
            if (!allowedFields.Any(field => predicate.Contains(field, StringComparison.OrdinalIgnoreCase)))
                return false;

            return true;
        }
    }
}
