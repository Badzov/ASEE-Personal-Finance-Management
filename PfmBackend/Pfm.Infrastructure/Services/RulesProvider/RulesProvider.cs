using Microsoft.Extensions.Configuration;
using Pfm.Application.Common;
using Pfm.Application.Interfaces;
using Pfm.Domain.Entities;
using Pfm.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Pfm.Infrastructure.Services.RulesProvider
{
    public sealed class RulesProvider : IRulesProvider
    {
        private readonly IReadOnlyList<AutoCategorizationRule> _rules;

        public RulesProvider(IConfiguration config)
        {
            try
            {
                var ruleSection = config.GetSection("rules");

                if (!ruleSection.Exists())
                {
                    throw new ValidationProblemException(new[] { new ValidationError("config", "rules-not-found", "Configuration section 'rules' is missing") });
                }

                var ruleDtos = ruleSection.Get<List<AutoCategorizationRuleDto>>()
                    ?? throw new ValidationProblemException(new[] { new ValidationError("config","rules-empty", "No rules defined in configuration") });

                _rules = ruleDtos.Select(dto => new AutoCategorizationRule(
                    dto.Id,
                    dto.Title,
                    dto.CatCode,
                    dto.Predicate
                )).ToList().AsReadOnly();

                // Domain-layer validation
                foreach (var rule in _rules) rule.Validate();
            }
            catch (JsonException ex) // Handles YAML parsing errors (IConfiguration uses JSON underneath)
            {
                throw new ValidationProblemException(new[] { new ValidationError("yaml", "invalid-yaml", $"Configuration parsing failed: {ex.Message}") });
            }
            catch (BusinessRuleException)
            {
                throw; 
            }
            catch (Exception ex)
            {
                throw new ValidationProblemException(new[] { new ValidationError("config", "rules-load-failed", $"Unexpected error loading rules: {ex.Message}") });
            }
        }

        public IReadOnlyList<AutoCategorizationRule> GetRules() => _rules;
    }
}
