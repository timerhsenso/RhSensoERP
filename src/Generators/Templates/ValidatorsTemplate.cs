// =============================================================================
// RHSENSOERP GENERATOR v3.7 - VALIDATORS TEMPLATE
// =============================================================================
using RhSensoERP.Generators.Models;
using System.Collections.Generic;
using System.Linq;

namespace RhSensoERP.Generators.Templates;

public static class ValidatorsTemplate
{
    public static string GenerateCreateValidator(EntityInfo info)
    {
        var validatableProps = info.CreateProperties
            .Where(p => !IsAuditOrTenantField(p.Name, info))
            .ToList();

        var rules = GenerateValidationRules(validatableProps, info);

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using FluentValidation;
using {{info.DtoNamespace}};

namespace {{info.ValidatorsNamespace}};

/// <summary>
/// Validator para criação de {{info.DisplayName}}.
/// </summary>
public sealed class Create{{info.EntityName}}RequestValidator
    : AbstractValidator<Create{{info.EntityName}}Request>
{
    public Create{{info.EntityName}}RequestValidator()
    {
{{rules}}
    }
}
""";
    }

    public static string GenerateUpdateValidator(EntityInfo info)
    {
        var validatableProps = info.UpdateProperties
            .Where(p => !IsAuditOrTenantField(p.Name, info))
            .ToList();

        var rules = GenerateValidationRules(validatableProps, info);

        return $$"""
{{info.FileHeader}}
using FluentValidation;
using {{info.DtoNamespace}};

namespace {{info.ValidatorsNamespace}};

/// <summary>
/// Validator para atualização de {{info.DisplayName}}.
/// </summary>
public sealed class Update{{info.EntityName}}RequestValidator
    : AbstractValidator<Update{{info.EntityName}}Request>
{
    public Update{{info.EntityName}}RequestValidator()
    {
{{rules}}
    }
}
""";
    }

    private static string GenerateValidationRules(
        IEnumerable<global::RhSensoERP.Generators.Models.PropertyInfo> properties,
        EntityInfo info)
    {
        var rules = new List<string>();

        foreach (global::RhSensoERP.Generators.Models.PropertyInfo prop in properties)
        {
            var ruleBuilder = new List<string>();

            if (prop.IsRequired || prop.RequiredOnCreate)
            {
                if (prop.IsString)
                    ruleBuilder.Add($".NotEmpty().WithMessage(\"{prop.DisplayName} é obrigatório\")");
                else
                    ruleBuilder.Add($".NotNull().WithMessage(\"{prop.DisplayName} é obrigatório\")");
            }

            if (prop.IsString && prop.MaxLength.HasValue)
            {
                ruleBuilder.Add($".MaximumLength({prop.MaxLength.Value}).WithMessage(\"{prop.DisplayName} deve ter no máximo {prop.MaxLength.Value} caracteres\")");
            }

            if (prop.IsString && prop.MinLength.HasValue)
            {
                ruleBuilder.Add($".MinimumLength({prop.MinLength.Value}).WithMessage(\"{prop.DisplayName} deve ter no mínimo {prop.MinLength.Value} caracteres\")");
            }

            if (ruleBuilder.Count > 0)
            {
                var rule = $"        RuleFor(x => x.{prop.Name})\n            {string.Join("\n            ", ruleBuilder)};";
                rules.Add(rule);
            }
        }

        return rules.Count > 0
            ? string.Join("\n\n", rules)
            : "        // Nenhuma regra de validação configurada";
    }

    private static bool IsAuditOrTenantField(string name, EntityInfo info)
    {
        if (name == info.CreatedAtField ||
            name == info.CreatedByField ||
            name == info.UpdatedAtField ||
            name == info.UpdatedByField)
            return true;

        var tenantFields = new[] { "TenantId", "IdSaas", "IdTenant" };
        return tenantFields.Contains(name);
    }
}