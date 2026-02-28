// =============================================================================
// RHSENSOERP GENERATOR v4.7 - ENTITY INFO MODEL
// =============================================================================
// Versão: 4.7 - ADICIONADO: Suporte a chave primária composta
//
// ✅ NOVIDADES v4.7:
// 1. HasCompositeKey - Flag indicando PK composta
// 2. CompositeKeyProperties - Lista de propriedades da PK composta
// 3. CompositeKeyColumns - Lista de colunas da PK composta
// 4. CompositeKeyTypes - Lista de tipos C# da PK composta
// 5. Helpers: PrimaryKeyParameterList, PrimaryKeyFindArgs, etc.
// =============================================================================

namespace RhSensoERP.Generators.Models;

/// <summary>
/// Informações extraídas de uma Entity para geração de CRUD.
/// </summary>
public class EntityInfo
{
    // =========================================================================
    // IDENTIFICAÇÃO
    // =========================================================================
    public string EntityName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PluralName { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string Schema { get; set; } = "dbo";
    public string Namespace { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string GeneratorVersion { get; set; } = string.Empty;
    public string FileHeader { get; set; } = string.Empty;

    // =========================================================================
    // CHAVE PRIMÁRIA (simples)
    // =========================================================================
    public string PrimaryKeyProperty { get; set; } = string.Empty;
    public string PrimaryKeyColumn { get; set; } = string.Empty;
    public string PrimaryKeyType { get; set; } = string.Empty;

    // =========================================================================
    // CHAVE PRIMÁRIA COMPOSTA - v4.7
    // =========================================================================

    /// <summary>
    /// Indica que a entidade possui chave primária composta (2+ colunas).
    /// Detectado via [PrimaryKey(nameof(Col1), nameof(Col2))] na classe.
    /// </summary>
    public bool HasCompositeKey { get; set; }

    /// <summary>
    /// Nomes das propriedades C# que compõem a PK composta.
    /// Ex: ["CdSistema", "CdFuncao"]
    /// </summary>
    public List<string> CompositeKeyProperties { get; set; } = new();

    /// <summary>
    /// Nomes das colunas SQL que compõem a PK composta.
    /// Ex: ["cdsistema", "cdfuncao"]
    /// </summary>
    public List<string> CompositeKeyColumns { get; set; } = new();

    /// <summary>
    /// Tipos C# das propriedades da PK composta.
    /// Ex: ["string", "string"]
    /// </summary>
    public List<string> CompositeKeyTypes { get; set; } = new();

    // =========================================================================
    // HELPERS PARA TEMPLATES - v4.7
    // =========================================================================

    /// <summary>
    /// Parâmetros para assinatura de método.
    /// Simples: "int id" | Composta: "string cdSistema, string cdFuncao"
    /// </summary>
    public string PrimaryKeyParameterList
    {
        get
        {
            if (!HasCompositeKey)
            {
                var paramName = ToCamelCase(PrimaryKeyProperty);
                return $"{PrimaryKeyType} {paramName}";
            }

            var parts = new List<string>();
            for (int i = 0; i < CompositeKeyProperties.Count; i++)
            {
                var type = CompositeKeyTypes[i];
                var name = ToCamelCase(CompositeKeyProperties[i]);
                parts.Add($"{type} {name}");
            }
            return string.Join(", ", parts);
        }
    }

    /// <summary>
    /// Argumentos para FindAsync / GetByIdAsync.
    /// Simples: "id" | Composta: "cdSistema, cdFuncao"
    /// </summary>
    public string PrimaryKeyArgumentList
    {
        get
        {
            if (!HasCompositeKey)
                return ToCamelCase(PrimaryKeyProperty);

            return string.Join(", ", CompositeKeyProperties.Select(p => ToCamelCase(p)));
        }
    }

    /// <summary>
    /// Args para EF FindAsync com new object[].
    /// Simples: "id" | Composta: "new object[] { cdSistema, cdFuncao }"
    /// </summary>
    public string PrimaryKeyFindArgs
    {
        get
        {
            if (!HasCompositeKey)
                return ToCamelCase(PrimaryKeyProperty);

            var args = string.Join(", ", CompositeKeyProperties.Select(p => ToCamelCase(p)));
            return $"new object[] {{ {args} }}";
        }
    }

    /// <summary>
    /// Expressão LINQ para comparar a PK.
    /// Simples: "e.Id == id" | Composta: "e.CdSistema == cdSistema && e.CdFuncao == cdFuncao"
    /// </summary>
    public string PrimaryKeyLinqFilter
    {
        get
        {
            if (!HasCompositeKey)
                return $"e.{PrimaryKeyProperty} == {ToCamelCase(PrimaryKeyProperty)}";

            var parts = CompositeKeyProperties
                .Select(p => $"e.{p} == {ToCamelCase(p)}");
            return string.Join(" && ", parts);
        }
    }

    /// <summary>
    /// Expressão LINQ para comparar a PK a partir de um objeto entity.
    /// Simples: "e.Id == entity.Id" | Composta: "e.CdSistema == entity.CdSistema && e.CdFuncao == entity.CdFuncao"
    /// </summary>
    public string PrimaryKeyLinqFilterFromEntity(string entityVarName = "entity")
    {
        if (!HasCompositeKey)
            return $"e.{PrimaryKeyProperty} == {entityVarName}.{PrimaryKeyProperty}";

        var parts = CompositeKeyProperties
            .Select(p => $"e.{p} == {entityVarName}.{p}");
        return string.Join(" && ", parts);
    }

    /// <summary>
    /// Expressão para OrderBy default.
    /// Simples: "e.Id" | Composta: "e.CdSistema" (primeira coluna da PK)
    /// </summary>
    public string PrimaryKeyDefaultOrderBy
    {
        get
        {
            if (!HasCompositeKey)
                return $"e.{PrimaryKeyProperty}";

            return $"e.{CompositeKeyProperties[0]}";
        }
    }

    /// <summary>
    /// Atribuição das PKs do request/command para a entity.
    /// Simples: "entity.Id = command.Id;" 
    /// Composta: "entity.CdSistema = command.CdSistema;\n    entity.CdFuncao = command.CdFuncao;"
    /// </summary>
    public string PrimaryKeyAssignFromCommand(string sourceVar = "command", string targetVar = "entity", string indent = "            ")
    {
        if (!HasCompositeKey)
            return $"{indent}{targetVar}.{PrimaryKeyProperty} = {sourceVar}.{PrimaryKeyProperty};";

        var lines = CompositeKeyProperties
            .Select(p => $"{indent}{targetVar}.{p} = {sourceVar}.{p};");
        return string.Join("\n", lines);
    }

    /// <summary>
    /// Campos da PK para retorno em Result (ex: em CreateCommand).
    /// Simples: "entity.Id" | Composta: "$\"{entity.CdSistema}|{entity.CdFuncao}\""
    /// </summary>
    public string PrimaryKeyResultExpression(string entityVar = "entity")
    {
        if (!HasCompositeKey)
            return $"{entityVar}.{PrimaryKeyProperty}";

        var interpolations = CompositeKeyProperties
            .Select(p => $"{{{entityVar}.{p}}}");
        return "$\"" + string.Join("|", interpolations) + "\"";
    }

    /// <summary>
    /// Parâmetros de rota para API controller.
    /// Simples: "{id}" | Composta: "{cdSistema}/{cdFuncao}"
    /// </summary>
    public string PrimaryKeyRouteTemplate
    {
        get
        {
            if (!HasCompositeKey)
                return $"{{{ToCamelCase(PrimaryKeyProperty)}}}";

            return string.Join("/", CompositeKeyProperties.Select(p => $"{{{ToCamelCase(p)}}}"));
        }
    }

    // =========================================================================
    // PERMISSÕES
    // =========================================================================
    public string CdSistema { get; set; } = string.Empty;
    public string CdFuncao { get; set; } = string.Empty;

    // =========================================================================
    // ROTAS API
    // =========================================================================
    public string ApiRoute { get; set; } = string.Empty;
    public string ApiGroup { get; set; } = string.Empty;
    public bool UsePluralRoute { get; set; } = false;
    public string ApiFullRoute { get; set; } = string.Empty;

    // =========================================================================
    // ✅ v4.7.1: CONTROLLER NAME - RESPEITA UsePluralRoute
    // =========================================================================
    /// <summary>
    /// Nome usado para Controller, Namespaces, Queries e Commands.
    /// UsePluralRoute = false (padrão) → EntityName (singular)
    /// UsePluralRoute = true           → PluralName (plural)
    /// </summary>
    public string ControllerName => UsePluralRoute ? PluralName : EntityName;

    // =========================================================================
    // AUDITORIA - v3.9.1 - NULLABLE CORRETO
    // =========================================================================
    public bool HasAuditFields { get; set; }
    public string CreatedAtField { get; set; } = string.Empty;
    public string CreatedByField { get; set; } = string.Empty;
    public string UpdatedAtField { get; set; } = string.Empty;
    public string UpdatedByField { get; set; } = string.Empty;

    public bool HasCreationAudit => !string.IsNullOrEmpty(CreatedAtField) || !string.IsNullOrEmpty(CreatedByField);
    public bool HasUpdateAudit => !string.IsNullOrEmpty(UpdatedAtField) || !string.IsNullOrEmpty(UpdatedByField);

    // =========================================================================
    // FLAGS DE GERAÇÃO
    // =========================================================================
    public bool GenerateDto { get; set; } = true;
    public bool GenerateRequests { get; set; } = true;
    public bool GenerateCommands { get; set; } = true;
    public bool GenerateQueries { get; set; } = true;
    public bool GenerateValidators { get; set; } = true;
    public bool GenerateRepository { get; set; } = true;
    public bool GenerateMapper { get; set; } = true;
    public bool GenerateEfConfig { get; set; } = true;
    public bool GenerateMetadata { get; set; } = true;
    public bool GenerateApiController { get; set; } = true;
    public bool GenerateWebController { get; set; } = false;
    public bool GenerateWebModels { get; set; } = false;
    public bool GenerateWebServices { get; set; } = false;
    public bool GenerateLookup { get; set; } = false;

    // =========================================================================
    // COMPORTAMENTOS
    // =========================================================================
    public bool SupportsBatchDelete { get; set; } = true;
    public bool ApiRequiresAuth { get; set; } = true;
    public bool IsLegacyTable { get; set; } = false;

    // =========================================================================
    // PROPRIEDADES
    // =========================================================================
    public List<PropertyInfo> Properties { get; set; } = new();

    public List<PropertyInfo> DtoProperties => Properties
        .Where(p => !p.IsNavigation)
        .ToList();

    public List<PropertyInfo> CreateProperties => Properties
        .Where(p => !p.IsPrimaryKey &&
                    !p.IsReadOnly &&
                    !p.IsNavigation &&
                    !IsAuditField(p.Name))
        .ToList();

    public List<PropertyInfo> UpdateProperties => Properties
        .Where(p => !p.IsPrimaryKey &&
                    !p.IsReadOnly &&
                    !p.IsNavigation &&
                    !IsAuditField(p.Name))
        .ToList();

    /// <summary>
    /// v4.7: Para entidades com PK composta, as propriedades da PK
    /// devem estar incluídas no Create (pois não são auto-geradas).
    /// </summary>
    public List<PropertyInfo> CreatePropertiesWithCompositeKey
    {
        get
        {
            if (!HasCompositeKey)
                return CreateProperties;

            // Inclui as PKs compostas + as demais propriedades editáveis
            var pkProps = Properties
                .Where(p => CompositeKeyProperties.Contains(p.Name))
                .ToList();

            var otherProps = Properties
                .Where(p => !p.IsPrimaryKey &&
                            !p.IsReadOnly &&
                            !p.IsNavigation &&
                            !IsAuditField(p.Name) &&
                            !CompositeKeyProperties.Contains(p.Name))
                .ToList();

            return pkProps.Concat(otherProps).ToList();
        }
    }

    // =========================================================================
    // NAVEGAÇÕES
    // =========================================================================
    public List<NavigationInfo> Navigations { get; set; } = new();
    public bool HasNavigations => Navigations.Count > 0;

    public List<NavigationInfo> ManyToOneNavigations =>
        Navigations.Where(n => n.RelationshipType == NavigationRelationshipType.ManyToOne).ToList();

    public List<NavigationInfo> OneToManyNavigations =>
        Navigations.Where(n => n.RelationshipType == NavigationRelationshipType.OneToMany).ToList();

    // =========================================================================
    // NAMESPACE BASE
    // =========================================================================
    public string BaseNamespace
    {
        get
        {
            var ns = Namespace;

            if (ns.EndsWith(".Domain.Entities"))
                return ns.Replace(".Domain.Entities", "");
            if (ns.EndsWith(".Core.Entities"))
                return ns.Replace(".Core.Entities", "");
            if (ns.EndsWith(".Entities"))
                return ns.Replace(".Entities", "");

            return ns;
        }
    }

    // =========================================================================
    // NAMESPACES — Usa PluralName (consistente com QueryTemplate/CommandTemplate)
    // =========================================================================
    public string DtoNamespace => $"{BaseNamespace}.Application.DTOs.{PluralName}";
    public string CommandsNamespace => $"{BaseNamespace}.Application.Features.{PluralName}.Commands";
    public string QueriesNamespace => $"{BaseNamespace}.Application.Features.{PluralName}.Queries";
    public string ValidatorsNamespace => $"{BaseNamespace}.Application.Validators.{PluralName}";
    public string RepositoryInterfaceNamespace => $"{BaseNamespace}.Core.Interfaces.Repositories";
    public string RepositoryImplNamespace => $"{BaseNamespace}.Infrastructure.Repositories";
    public string MapperNamespace => $"{BaseNamespace}.Application.Mappings";
    public string EfConfigNamespace => $"{BaseNamespace}.Infrastructure.Persistence.Configurations";
    public string ApiControllerNamespace => $"{BaseNamespace}.WebApi.Controllers";
    public string WebControllerNamespace => $"RhSensoERP.Web.Controllers.{ModuleName}";
    public string WebModelsNamespace => $"RhSensoERP.Web.Models.{ModuleName}";
    public string WebServicesNamespace => $"RhSensoERP.Web.Services.{ModuleName}";
    public string ModuleNamespace => BaseNamespace;

    // =========================================================================
    // DbContext
    // =========================================================================
    public string DbContextName => $"{ModuleName}DbContext";

    public string DbContextNamespace
    {
        get
        {
            var baseNs = BaseNamespace;
            var persistenceNs = $"{baseNs}.Infrastructure.Persistence";
            return $"{persistenceNs}.Contexts";
        }
    }

    // =========================================================================
    // MÉTODOS AUXILIARES
    // =========================================================================
    private bool IsAuditField(string propertyName)
    {
        return propertyName == CreatedAtField ||
               propertyName == CreatedByField ||
               propertyName == UpdatedAtField ||
               propertyName == UpdatedByField;
    }

    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return char.ToLowerInvariant(value[0]) + value.Substring(1);
    }
}