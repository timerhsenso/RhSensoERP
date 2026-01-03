// =============================================================================
// RHSENSOERP GENERATOR v3.5 - GENERATE CRUD ATTRIBUTE
// =============================================================================
// Arquivo: src/Shared/Core/Attributes/GenerateCrudAttribute.cs
// Versão: 3.5 - Adicionado UsePluralRoute (padrão: singular)
// =============================================================================

namespace RhSensoERP.Shared.Core.Attributes;

/// <summary>
/// Atributo que marca uma Entity para geração automática de CRUD.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class GenerateCrudAttribute : Attribute
{
    // =========================================================================
    // IDENTIFICAÇÃO
    // =========================================================================

    /// <summary>
    /// Nome da tabela no banco de dados.
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Schema da tabela (padrão: "dbo").
    /// </summary>
    public string Schema { get; set; } = "dbo";

    /// <summary>
    /// Nome amigável para exibição.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    // =========================================================================
    // PERMISSÕES
    // =========================================================================

    /// <summary>
    /// Código do sistema (ex: "SEG", "RHU", "SGT").
    /// </summary>
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Código da função para permissões (ex: "SEG_FM_TSISTEMA").
    /// </summary>
    public string CdFuncao { get; set; } = string.Empty;

    // =========================================================================
    // ROTAS API
    // =========================================================================

    /// <summary>
    /// Rota base da API (ex: "integracoes/dispositivo").
    /// Se vazio, será gerada automaticamente usando EntityName ou PluralName
    /// conforme a flag UsePluralRoute.
    /// </summary>
    public string ApiRoute { get; set; } = string.Empty;

    /// <summary>
    /// Nome do grupo no Swagger.
    /// </summary>
    public string ApiGroup { get; set; } = string.Empty;

    /// <summary>
    /// Se TRUE, pluraliza o nome da entidade na rota da API.
    /// PADRÃO: FALSE - Usa nome singular (ex: /api/integracoes/dispositivo)
    /// Se TRUE: Usa nome plural (ex: /api/integracoes/dispositivos)
    /// </summary>
    public bool UsePluralRoute { get; set; } = false;

    // =========================================================================
    // CHAVE PRIMÁRIA (override)
    // =========================================================================

    /// <summary>
    /// Nome da propriedade que é a PK (se não usar [Key]).
    /// </summary>
    public string PrimaryKeyProperty { get; set; } = string.Empty;

    /// <summary>
    /// Tipo C# da PK (se não puder ser inferido).
    /// </summary>
    public string PrimaryKeyType { get; set; } = string.Empty;

    // =========================================================================
    // FLAGS DE GERAÇÃO - BACKEND
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

    // =========================================================================
    // FLAGS DE GERAÇÃO - API/WEB
    // =========================================================================

    // No arquivo GenerateCrudAttribute.cs, adicione:
    public bool GenerateLookup { get; set; } = false;

    public bool GenerateApiController { get; set; } = true;
    public bool GenerateWebController { get; set; } = false;
    public bool GenerateWebModels { get; set; } = false;
    public bool GenerateWebServices { get; set; } = false;

    // =========================================================================
    // COMPORTAMENTOS
    // =========================================================================

    public bool SupportsBatchDelete { get; set; } = true;
    public bool ApiRequiresAuth { get; set; } = true;

    // =========================================================================
    // AUDITORIA
    // =========================================================================

    /// <summary>
    /// Indica que é tabela legada com convenções antigas.
    /// Mantido para compatibilidade.
    /// </summary>
    public bool IsLegacyTable { get; set; } = false;

    /// <summary>
    /// Se TRUE, o generator adiciona propriedades de auditoria base (CreatedAt, CreatedBy, etc.)
    /// caso não existam na entidade.
    /// 
    /// PADRÃO: FALSE - O generator respeita as propriedades existentes na entidade.
    /// 
    /// Use TRUE apenas para entidades que herdam de BaseEntity e precisam dessas propriedades.
    /// Entidades com padrão Aud_* (Aud_CreatedAt, Aud_IdUsuarioCadastro) NÃO precisam disso.
    /// </summary>
    public bool InjectBaseAuditProperties { get; set; } = false;
}