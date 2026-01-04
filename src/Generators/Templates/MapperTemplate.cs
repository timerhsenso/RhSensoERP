// =============================================================================
// RHSENSOERP GENERATOR v4.6 - MAPPER TEMPLATE
// =============================================================================
// v4.6: ADICIONADO - Mapeamento automático de navegações
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

        // ✅ v4.6 NOVO: Gera mapeamento de navegações
        var navigationMappings = GenerateNavigationMappings(info);

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
        CreateMap<{{info.EntityName}}, {{info.EntityName}}Dto>(){{navigationMappings}};

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

    // =========================================================================
    // ✅ v4.6 NOVO: MAPEAMENTO DE NAVEGAÇÕES
    // =========================================================================

    /// <summary>
    /// Gera mapeamentos de navegações.
    /// Ex: .ForMember(dest => dest.FornecedorRazaoSocial, opt => opt.MapFrom(src => src.Fornecedor!.RazaoSocial))
    /// </summary>
    private static string GenerateNavigationMappings(EntityInfo info)
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
            var dtoProp = nav.DtoPropertyNameComputed;
            var navName = nav.Name;
            var displayProp = nav.DisplayProperty;

            // Usa ! (null-forgiving) porque sabemos que será populado pelo Include
            lines.Add($"            .ForMember(dest => dest.{dtoProp}, opt => opt.MapFrom(src => src.{navName}!.{displayProp}))");
        }

        return "\n" + string.Join("\n", lines);
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

        // ✅ v4.6: Ignora propriedades de navegação
        var navIgnores = GenerateNavigationIgnores(info);
        ignores.AddRange(navIgnores);

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

        // ✅ v4.6: Ignora propriedades de navegação
        var navIgnores = GenerateNavigationIgnores(info);
        ignores.AddRange(navIgnores);

        return string.Join("\n", ignores);
    }

    /// <summary>
    /// Gera ignores para propriedades de navegação (elas não existem na entidade).
    /// </summary>
    private static List<string> GenerateNavigationIgnores(EntityInfo info)
    {
        var ignores = new List<string>();

        var navWithDisplay = info.Navigations
            .Where(n => n.HasNavigationDisplay &&
                       n.RelationshipType == NavigationRelationshipType.ManyToOne)
            .ToList();

        foreach (var nav in navWithDisplay)
        {
            // Ignora a navegação em si (ex: Fornecedor)
            ignores.Add($"            .ForMember(dest => dest.{nav.Name}, opt => opt.Ignore())");
        }

        return ignores;
    }
}