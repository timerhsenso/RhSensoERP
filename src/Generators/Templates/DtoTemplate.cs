// =============================================================================
// RHSENSOERP GENERATOR v3.9 - DTO TEMPLATE
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
        foreach (var p in info.DtoProperties)
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