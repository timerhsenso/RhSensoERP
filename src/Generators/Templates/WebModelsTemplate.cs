// =============================================================================
// RHSENSOERP GENERATOR v3.2 - WEB MODELS TEMPLATE
// =============================================================================
// v3.2: CORRIGIDO - PK texto/numérica não-identity incluída no CreateDto
// =============================================================================
using RhSensoERP.Generators.Models;
using System.Collections.Generic;
using System.Linq;

namespace RhSensoERP.Generators.Templates;

public static class WebModelsTemplate
{
    public static string GenerateDto(EntityInfo info)
    {
        var props = new List<string>();
        foreach (global::RhSensoERP.Generators.Models.PropertyInfo p in info.DtoProperties)
        {
            props.Add($"    public {p.Type} {p.Name} {{ get; set; }}{GetDefaultValue(p)}");
        }

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;

namespace {{info.WebModelsNamespace}};

/// <summary>
/// DTO para exibição de {{info.DisplayName}} no frontend.
/// </summary>
public sealed class {{info.EntityName}}Dto
{
{{string.Join("\n", props)}}
}
""";
    }

    public static string GenerateCreateDto(EntityInfo info)
    {
        var createProps = info.CreateProperties.ToList();

        // =====================================================================
        // v3.2: Inclui PK no CreateDto quando não é auto-gerada
        // PK Identity/Guid = auto-gerada → NÃO incluir
        // PK texto/numérica não-identity = informada pelo usuário → INCLUIR
        // PK composta = TODAS as colunas da PK devem ser incluídas
        // =====================================================================
        if (info.HasCompositeKey)
        {
            var pkProps = info.Properties.Where(p => p.IsPrimaryKey).Reverse().ToList();
            foreach (var pkProp in pkProps)
            {
                if (!createProps.Any(p => p.Name == pkProp.Name))
                {
                    createProps.Insert(0, pkProp);
                }
            }
        }
        else if (!info.HasCompositeKey)
        {
            var pkProp = info.Properties.FirstOrDefault(p => p.IsPrimaryKey);
            if (pkProp != null && !pkProp.IsIdentity && !pkProp.IsGuid)
            {
                if (!createProps.Any(p => p.Name == pkProp.Name))
                {
                    createProps.Insert(0, pkProp);
                }
            }
        }

        var props = new List<string>();
        foreach (global::RhSensoERP.Generators.Models.PropertyInfo p in createProps)
        {
            props.Add($"    public {p.Type} {p.Name} {{ get; set; }}{GetDefaultValue(p)}");
        }

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;

namespace {{info.WebModelsNamespace}};

/// <summary>
/// DTO para criação de {{info.DisplayName}}.
/// </summary>
public sealed class Create{{info.EntityName}}Dto
{
{{string.Join("\n", props)}}
}
""";
    }

    public static string GenerateUpdateDto(EntityInfo info)
    {
        var props = new List<string>();
        foreach (global::RhSensoERP.Generators.Models.PropertyInfo p in info.UpdateProperties)
        {
            props.Add($"    public {p.Type} {p.Name} {{ get; set; }}{GetDefaultValue(p)}");
        }

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;

namespace {{info.WebModelsNamespace}};

/// <summary>
/// DTO para atualização de {{info.DisplayName}}.
/// </summary>
public sealed class Update{{info.EntityName}}Dto
{
{{string.Join("\n", props)}}
}
""";
    }

    public static string GenerateListViewModel(EntityInfo info)
    {
        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using RhSensoERP.Web.Models.Base;

namespace {{info.WebModelsNamespace}};

/// <summary>
/// ViewModel para a página de listagem de {{info.DisplayName}}.
/// Contém informações de permissões do usuário para controle de botões.
/// </summary>
public sealed class {{info.PluralName}}ListViewModel : BaseListViewModel
{
    public string PageTitle { get; set; } = "{{info.DisplayName}}";
    public string PageSubtitle { get; set; } = "Gerenciamento de {{info.DisplayName}}";
    public string CdFuncao { get; set; } = "{{info.CdFuncao}}";
    public string CdSistema { get; set; } = "{{info.CdSistema}}";
}
""";
    }

    private static string GetDefaultValue(global::RhSensoERP.Generators.Models.PropertyInfo prop)
    {
        if (!string.IsNullOrEmpty(prop.DefaultValue))
            return $" = {prop.DefaultValue};";

        if (prop.IsString && !prop.IsNullable)
            return " = string.Empty;";

        return string.Empty;
    }
}