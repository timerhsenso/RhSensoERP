// =============================================================================
// RHSENSOERP GENERATOR v3.9.1 - ENTITY INFO MODEL
// =============================================================================
// Versão: 3.9.1 - CORREÇÃO: Nullable reference types
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
    // CHAVE PRIMÁRIA
    // =========================================================================
    public string PrimaryKeyProperty { get; set; } = string.Empty;
    public string PrimaryKeyColumn { get; set; } = string.Empty;
    public string PrimaryKeyType { get; set; } = string.Empty;

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
    // NAMESPACES
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
}