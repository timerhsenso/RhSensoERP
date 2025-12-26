// =============================================================================
// RHSENSOERP GENERATOR v3.7 - MAPPER TEMPLATE
// =============================================================================
// Arquivo: src/Generators/Templates/MapperTemplate.cs
// Versão: 3.7 - Ignora campos protegidos (PK, TenantId, Auditoria)
// =============================================================================
using RhSensoERP.Generators.Models;
using System.Collections.Generic;
using System.Linq;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de AutoMapper Profile.
/// </summary>
public static class MapperTemplate
{
    /// <summary>
    /// Gera o Profile do AutoMapper.
    /// </summary>
    public static string GenerateProfile(EntityInfo info)
    {
        var entityNs = info.Namespace;

        // ✅ Gera ignores para campos protegidos
        var createIgnores = GenerateCreateIgnores(info);
        var updateIgnores = GenerateUpdateIgnores(info);

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using AutoMapper;
using {{entityNs}};
using {{info.DtoNamespace}};

namespace {{info.MapperNamespace}};

/// <summary>
/// Perfil de mapeamento do AutoMapper para {{info.DisplayName}}.
/// </summary>
public sealed class {{info.EntityName}}Profile : Profile
{
    public {{info.EntityName}}Profile()
    {
        // Entity → DTO
        CreateMap<{{info.EntityName}}, {{info.EntityName}}Dto>();

        // CreateRequest → Entity
        CreateMap<Create{{info.EntityName}}Request, {{info.EntityName}}>()
{{createIgnores}}
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // UpdateRequest → Entity
        CreateMap<Update{{info.EntityName}}Request, {{info.EntityName}}>()
{{updateIgnores}}
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
""";
    }

    /// <summary>
    /// Gera ignores para CreateRequest.
    /// </summary>
    private static string GenerateCreateIgnores(EntityInfo info)
    {
        var ignores = new List<string>();

        // Ignora PK
        ignores.Add($"            .ForMember(dest => dest.{info.PrimaryKeyProperty}, opt => opt.Ignore())");

        // Ignora TenantId (se não for legada)
        if (!info.IsLegacyTable)
        {
            var tenantFields = new[] { "TenantId", "IdSaas", "IdTenant" };
            foreach (var field in tenantFields)
            {
                if (info.Properties.Any(p => p.Name == field))
                {
                    ignores.Add($"            .ForMember(dest => dest.{field}, opt => opt.Ignore())");
                }
            }
        }

        // Ignora campos de auditoria
        if (!string.IsNullOrEmpty(info.CreatedAtField))
            ignores.Add($"            .ForMember(dest => dest.{info.CreatedAtField}, opt => opt.Ignore())");

        if (!string.IsNullOrEmpty(info.CreatedByField))
            ignores.Add($"            .ForMember(dest => dest.{info.CreatedByField}, opt => opt.Ignore())");

        if (!string.IsNullOrEmpty(info.UpdatedAtField))
            ignores.Add($"            .ForMember(dest => dest.{info.UpdatedAtField}, opt => opt.Ignore())");

        if (!string.IsNullOrEmpty(info.UpdatedByField))
            ignores.Add($"            .ForMember(dest => dest.{info.UpdatedByField}, opt => opt.Ignore())");

        return string.Join("\n", ignores);
    }

    /// <summary>
    /// Gera ignores para UpdateRequest.
    /// </summary>
    private static string GenerateUpdateIgnores(EntityInfo info)
    {
        var ignores = new List<string>();

        // Ignora PK
        ignores.Add($"            .ForMember(dest => dest.{info.PrimaryKeyProperty}, opt => opt.Ignore())");

        // Ignora TenantId
        if (!info.IsLegacyTable)
        {
            var tenantFields = new[] { "TenantId", "IdSaas", "IdTenant" };
            foreach (var field in tenantFields)
            {
                if (info.Properties.Any(p => p.Name == field))
                {
                    ignores.Add($"            .ForMember(dest => dest.{field}, opt => opt.Ignore())");
                }
            }
        }

        // Ignora TODOS campos de auditoria (criação + update)
        if (!string.IsNullOrEmpty(info.CreatedAtField))
            ignores.Add($"            .ForMember(dest => dest.{info.CreatedAtField}, opt => opt.Ignore())");

        if (!string.IsNullOrEmpty(info.CreatedByField))
            ignores.Add($"            .ForMember(dest => dest.{info.CreatedByField}, opt => opt.Ignore())");

        if (!string.IsNullOrEmpty(info.UpdatedAtField))
            ignores.Add($"            .ForMember(dest => dest.{info.UpdatedAtField}, opt => opt.Ignore())");

        if (!string.IsNullOrEmpty(info.UpdatedByField))
            ignores.Add($"            .ForMember(dest => dest.{info.UpdatedByField}, opt => opt.Ignore())");

        return string.Join("\n", ignores);
    }
}