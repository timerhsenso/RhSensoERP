// =============================================================================
// RHSENSOERP GENERATOR v4.6 - DTO TEMPLATE
// =============================================================================
// v4.6: ADICIONADO - Propriedades de navegação (ex: FornecedorRazaoSocial)
// =============================================================================
using RhSensoERP.Generators.Models;
using System.Collections.Generic;
using System.Linq;

namespace RhSensoERP.Generators.Templates;

public static class DtoTemplate
{
    public static string GenerateDto(EntityInfo info)
    {
        var props = new List<string>();

        // ✅ Propriedades escalares normais
        foreach (var p in info.DtoProperties)
        {
            props.Add($"    public {p.Type} {p.Name} {{ get; set; }}{GetDefaultValue(p)}");
        }

        // ✅ v4.6 NOVO: Propriedades de navegação
        var navigationProps = GenerateNavigationProperties(info);
        if (!string.IsNullOrEmpty(navigationProps))
        {
            props.Add("");
            props.Add("    // =========================================================================");
            props.Add("    // PROPRIEDADES DE NAVEGAÇÃO (campos de entidades relacionadas)");
            props.Add("    // =========================================================================");
            props.Add(navigationProps);
        }

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using {{info.Namespace}};

namespace {{info.DtoNamespace}};

/// <summary>
/// DTO para leitura de {{info.DisplayName}}.
/// </summary>
public sealed class {{info.EntityName}}Dto
{
{{string.Join("\n", props)}}
}
""";
    }

    public static string GenerateCreateRequest(EntityInfo info)
    {
        var filteredProps = info.CreateProperties
            .Where(p => !IsAuditOrTenantField(p.Name, info))
            .ToList();

        var props = new List<string>();
        foreach (var p in filteredProps)
        {
            props.Add($"    public {p.Type} {p.Name} {{ get; set; }}{GetDefaultValue(p)}");
        }

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using {{info.Namespace}};

namespace {{info.DtoNamespace}};

/// <summary>
/// Request para criação de {{info.DisplayName}}.
/// </summary>
public sealed class Create{{info.EntityName}}Request
{
{{string.Join("\n", props)}}
}
""";
    }

    public static string GenerateUpdateRequest(EntityInfo info)
    {
        var filteredProps = info.UpdateProperties
            .Where(p => !IsAuditOrTenantField(p.Name, info))
            .ToList();

        var props = new List<string>();
        foreach (var p in filteredProps)
        {
            props.Add($"    public {p.Type} {p.Name} {{ get; set; }}{GetDefaultValue(p)}");
        }

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using {{info.Namespace}};

namespace {{info.DtoNamespace}};

/// <summary>
/// Request para atualização de {{info.DisplayName}}.
/// </summary>
public sealed class Update{{info.EntityName}}Request
{
{{string.Join("\n", props)}}
}
""";
    }

    // =========================================================================
    // ✅ v4.6 NOVO: GERAÇÃO DE PROPRIEDADES DE NAVEGAÇÃO
    // =========================================================================

    /// <summary>
    /// Gera propriedades de navegação baseadas no atributo [NavigationDisplay].
    /// Ex: public string? FornecedorRazaoSocial { get; set; }
    /// </summary>
    private static string GenerateNavigationProperties(EntityInfo info)
    {
        var navProps = info.Navigations
            .Where(n => n.HasNavigationDisplay &&
                       n.RelationshipType == NavigationRelationshipType.ManyToOne)
            .ToList();

        if (!navProps.Any())
            return "";

        var lines = new List<string>();

        foreach (var nav in navProps)
        {
            var propName = nav.DtoPropertyNameComputed;
            var displayProp = nav.DisplayProperty;

            lines.Add($"    /// <summary>");
            lines.Add($"    /// Campo '{displayProp}' da navegação {nav.Name}.");
            lines.Add($"    /// </summary>");
            lines.Add($"    public string? {propName} {{ get; set; }}");
        }

        return string.Join("\n", lines);
    }

    private static string GetDefaultValue(RhSensoERP.Generators.Models.PropertyInfo prop)
    {
        if (!string.IsNullOrEmpty(prop.DefaultValue))
            return $" = {prop.DefaultValue};";

        if (prop.IsString && !prop.IsNullable)
            return " = string.Empty;";

        return string.Empty;
    }

    private static bool IsAuditOrTenantField(string name, EntityInfo info)
    {
        if (name == info.CreatedAtField ||
            name == info.CreatedByField ||
            name == info.UpdatedAtField ||
            name == info.UpdatedByField)
            return true;

        var tenantFields = new[] { "TenantId", "IdSaaS", "IdSaas", "IdTenant" };
        return tenantFields.Contains(name);
    }
}