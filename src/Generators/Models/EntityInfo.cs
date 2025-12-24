// =============================================================================
// RHSENSOERP GENERATOR v3.6.2 - ENTITY INFO MODEL
// =============================================================================
// Arquivo: src/Generators/Models/EntityInfo.cs
// Versão: 3.6.2 - BaseNamespace agora é PÚBLICO
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
    
    /// <summary>
    /// Versão do gerador que criou este arquivo.
    /// Útil para debug.
    /// </summary>
    public string GeneratorVersion { get; set; } = string.Empty;

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
    // AUDITORIA - v3.6
    // =========================================================================
    public bool HasAuditFields { get; set; }
    public string? CreatedAtField { get; set; }
    public string? CreatedByField { get; set; }
    public string? UpdatedAtField { get; set; }
    public string? UpdatedByField { get; set; }

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
    // NAMESPACE BASE - v3.6.2 - AGORA É PÚBLICO
    // =========================================================================

    /// <summary>
    /// Calcula o namespace base removendo sufixos de entidade.
    /// 
    /// Exemplos:
    /// - RhSensoERP.Identity.Core.Entities → RhSensoERP.Identity
    /// - RhSensoERP.Modules.XXX.Core.Entities → RhSensoERP.Modules.XXX
    /// </summary>
    public string BaseNamespace
    {
        get
        {
            var ns = Namespace;

            // Remove sufixos de entidades
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
    // NAMESPACES - v3.6.2 CORRIGIDOS
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
    // DbContext - v3.6.2
    // =========================================================================
    public string DbContextName => $"{ModuleName}DbContext";

    /// <summary>
    /// Calcula o namespace correto do DbContext.
    /// 
    /// REGRA:
    /// - Identity: RhSensoERP.Identity.Infrastructure.Persistence.Contexts (COM Contexts)
    /// - Modules: RhSensoERP.Modules.XXX.Infrastructure.Persistence.Contexts (COM Contexts)
    /// 
    /// Exemplos:
    /// - RhSensoERP.Identity.Core.Entities → RhSensoERP.Identity.Infrastructure.Persistence.Contexts
    /// - RhSensoERP.Modules.TreinamentoDesenvolvimento.Core.Entities → RhSensoERP.Modules.TreinamentoDesenvolvimento.Infrastructure.Persistence.Contexts
    /// </summary>
    public string DbContextNamespace
    {
        get
        {
            var baseNs = BaseNamespace;
            var persistenceNs = $"{baseNs}.Infrastructure.Persistence";

            // ✅ TODOS COM Contexts agora (Identity também foi padronizado)
            return $"{persistenceNs}.Contexts";
        }
    }

    // =========================================================================
    // MÉTODOS AUXILIARES
    // =========================================================================

    /// <summary>
    /// Verifica se um campo é de auditoria.
    /// </summary>
    private bool IsAuditField(string propertyName)
    {
        return propertyName == CreatedAtField ||
               propertyName == CreatedByField ||
               propertyName == UpdatedAtField ||
               propertyName == UpdatedByField;
    }
}