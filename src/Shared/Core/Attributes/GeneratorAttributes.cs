// =============================================================================
// RHSENSOERP - ATRIBUTOS PARA SOURCE GENERATOR
// =============================================================================
// Arquivo: src/Shared/Core/Attributes/GeneratorAttributes.cs
// Versão: 3.6 - Adicionado HasDatabaseTriggersAttribute
// =============================================================================
// IMPORTANTE: Este arquivo deve estar sincronizado com:
// src/Generators/Attributes/GenerateCrudAttribute.cs
// =============================================================================

namespace RhSensoERP.Shared.Core.Attributes;

/// <summary>
/// Atributo que marca uma Entity para geração automática de código CRUD.
/// Gera: DTOs, Commands, Queries, Validators, Repository, Mapper, EF Config,
/// API Controller, Web Controller, Web Models, Web Services e MetadataProvider.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GenerateCrudAttribute : Attribute
{
    // =========================================================================
    // CONFIGURAÇÕES BÁSICAS
    // =========================================================================

    /// <summary>
    /// Nome da tabela no banco de dados.
    /// Exemplo: "tsistema", "tfunc1", "tusuario"
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Schema da tabela (padrão: "dbo").
    /// </summary>
    public string Schema { get; set; } = "dbo";

    /// <summary>
    /// Nome amigável da entidade para exibição.
    /// Exemplo: "Sistema", "Função", "Usuário"
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    // =========================================================================
    // CONFIGURAÇÕES DE MÓDULO E PERMISSÕES
    // =========================================================================

    /// <summary>
    /// Código do sistema/módulo ao qual esta entidade pertence.
    /// Exemplos: "SEG" (Segurança), "RHU" (RH), "FIN" (Financeiro)
    /// </summary>
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Código da função/tela no sistema de permissões legado.
    /// Exemplo: "SEG_FM_TSISTEMA", "RHU_FM_FUNCIONARIO"
    /// </summary>
    public string CdFuncao { get; set; } = string.Empty;

    // =========================================================================
    // CONFIGURAÇÕES DE ROTA E API
    // =========================================================================

    /// <summary>
    /// Rota base da API (sem prefixo api/).
    /// Se vazio, será gerado automaticamente: {module}/{entity} ou {module}/{entityPlural}
    /// conforme a flag UsePluralRoute.
    /// </summary>
    public string ApiRoute { get; set; } = string.Empty;

    /// <summary>
    /// Nome do grupo no Swagger/OpenAPI.
    /// </summary>
    public string ApiGroup { get; set; } = string.Empty;

    /// <summary>
    /// Se TRUE, pluraliza o nome da entidade na rota da API.
    /// PADRÃO: FALSE - Usa nome singular (ex: /api/integracoes/dispositivo)
    /// Se TRUE: Usa nome plural (ex: /api/integracoes/dispositivos)
    /// </summary>
    public bool UsePluralRoute { get; set; } = false;

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

    /// <summary>
    /// Gera o MetadataProvider para UI dinâmica.
    /// </summary>
    public bool GenerateMetadata { get; set; } = true;

    // =========================================================================
    // FLAGS DE GERAÇÃO - API/WEB (FALSE por padrão)
    // =========================================================================

    public bool GenerateLookup { get; set; } = false;

    public bool GenerateApiController { get; set; } = false;
    public bool GenerateWebController { get; set; } = false;
    public bool GenerateWebModels { get; set; } = false;
    public bool GenerateWebServices { get; set; } = false;

    public bool ApiRequiresAuth { get; set; } = true;
    public bool SupportsBatchDelete { get; set; } = true;

    // =========================================================================
    // CONFIGURAÇÕES DE TABELA LEGADA
    // =========================================================================

    /// <summary>
    /// Indica se é tabela legada (sem BaseEntity).
    /// </summary>
    public bool IsLegacyTable { get; set; } = false;

    /// <summary>
    /// Se TRUE, o generator adiciona propriedades de auditoria base (CreatedAt, CreatedBy, etc.)
    /// caso não existam na entidade.
    /// PADRÃO: FALSE - O generator respeita as propriedades existentes na entidade.
    /// </summary>
    public bool InjectBaseAuditProperties { get; set; } = false;

    public string PrimaryKeyProperty { get; set; } = string.Empty;
    public string PrimaryKeyType { get; set; } = string.Empty;

    // =========================================================================
    // CONFIGURAÇÕES DE UI (v3.1)
    // =========================================================================

    /// <summary>
    /// Ícone da entidade (FontAwesome).
    /// Exemplo: "fa-users", "fa-building"
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// Tamanho padrão da página no DataTable.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Campo padrão para ordenação.
    /// </summary>
    public string DefaultSortField { get; set; } = string.Empty;

    /// <summary>
    /// Direção padrão de ordenação ("asc" ou "desc").
    /// </summary>
    public string DefaultSortDirection { get; set; } = "asc";

    /// <summary>
    /// Permite exportação para Excel.
    /// </summary>
    public bool CanExport { get; set; } = true;

    /// <summary>
    /// Permite importação de Excel.
    /// </summary>
    public bool CanImport { get; set; } = false;

    /// <summary>
    /// Permite impressão.
    /// </summary>
    public bool CanPrint { get; set; } = true;
}

// =============================================================================
// ATRIBUTOS DE CAMPO - BÁSICOS
// =============================================================================

/// <summary>
/// Define nome de exibição de um campo.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class FieldDisplayNameAttribute : Attribute
{
    public string DisplayName { get; }
    public FieldDisplayNameAttribute(string displayName) => DisplayName = displayName;
}

/// <summary>
/// Marca campo para não incluir no DTO de leitura.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class ExcludeFromDtoAttribute : Attribute { }

/// <summary>
/// Marca campo como somente leitura.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class ReadOnlyFieldAttribute : Attribute { }

/// <summary>
/// Marca campo como obrigatório na criação.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class RequiredOnCreateAttribute : Attribute { }

/// <summary>
/// Configura validação customizada.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
public sealed class ValidationRuleAttribute : Attribute
{
    public string RuleType { get; }
    public string? Parameter { get; set; }
    public string? ErrorMessage { get; set; }

    public ValidationRuleAttribute(string ruleType) => RuleType = ruleType;
}

// =============================================================================
// ATRIBUTOS DE UI - LISTAGEM (DataTable)
// =============================================================================

/// <summary>
/// Configura o comportamento do campo na listagem (DataTable).
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class ListConfigAttribute : Attribute
{
    /// <summary>
    /// Exibir na listagem. Padrão: true
    /// </summary>
    public bool Show { get; set; } = true;

    /// <summary>
    /// Ordem de exibição na listagem.
    /// </summary>
    public int Order { get; set; } = 0;

    /// <summary>
    /// Largura da coluna (em pixels ou percentual).
    /// Exemplo: "150px", "20%"
    /// </summary>
    public string Width { get; set; } = string.Empty;

    /// <summary>
    /// Permite ordenação por este campo.
    /// </summary>
    public bool Sortable { get; set; } = true;

    /// <summary>
    /// Permite busca/filtro por este campo.
    /// </summary>
    public bool Searchable { get; set; } = true;

    /// <summary>
    /// Formato de exibição do valor.
    /// Exemplos: "date", "datetime", "currency", "percentage", "boolean"
    /// </summary>
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// Alinhamento: "left", "center", "right"
    /// </summary>
    public string Align { get; set; } = "left";
}

// =============================================================================
// ATRIBUTOS DE UI - FORMULÁRIO
// =============================================================================

/// <summary>
/// Configura o comportamento do campo no formulário.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class FormConfigAttribute : Attribute
{
    /// <summary>
    /// Exibir no formulário. Padrão: true
    /// </summary>
    public bool Show { get; set; } = true;

    /// <summary>
    /// Exibir no formulário de criação.
    /// </summary>
    public bool ShowOnCreate { get; set; } = true;

    /// <summary>
    /// Exibir no formulário de edição.
    /// </summary>
    public bool ShowOnEdit { get; set; } = true;

    /// <summary>
    /// Ordem de exibição no formulário.
    /// </summary>
    public int Order { get; set; } = 0;

    /// <summary>
    /// Grupo/seção do formulário.
    /// Exemplo: "Dados Pessoais", "Endereço"
    /// </summary>
    public string Group { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de input HTML: "text", "textarea", "select", "date", "datetime", 
    /// "time", "number", "email", "tel", "url", "password", "checkbox", "radio", "hidden"
    /// </summary>
    public string InputType { get; set; } = "text";

    /// <summary>
    /// Texto placeholder do campo.
    /// </summary>
    public string Placeholder { get; set; } = string.Empty;

    /// <summary>
    /// Texto de ajuda exibido abaixo do campo.
    /// </summary>
    public string HelpText { get; set; } = string.Empty;

    /// <summary>
    /// Máscara de input (para campos formatados).
    /// Exemplo: "000.000.000-00" (CPF)
    /// </summary>
    public string Mask { get; set; } = string.Empty;

    /// <summary>
    /// Número de linhas para textarea.
    /// </summary>
    public int Rows { get; set; } = 3;

    /// <summary>
    /// Número de colunas do grid (1-12, Bootstrap).
    /// </summary>
    public int ColSize { get; set; } = 6;

    /// <summary>
    /// Ícone do campo (FontAwesome).
    /// </summary>
    public string Icon { get; set; } = string.Empty;
}

// =============================================================================
// ATRIBUTOS DE UI - LOOKUP/RELACIONAMENTO
// =============================================================================

/// <summary>
/// Configura um campo como lookup (dropdown/autocomplete) para relacionamentos.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class LookupAttribute : Attribute
{
    /// <summary>
    /// Endpoint da API para buscar as opções.
    /// Exemplo: "/api/rhu/bancos"
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Nome do campo que contém o valor (ID).
    /// </summary>
    public string ValueField { get; set; } = "Id";

    /// <summary>
    /// Nome do campo que contém o texto de exibição.
    /// </summary>
    public string TextField { get; set; } = "Nome";

    /// <summary>
    /// Permite busca/autocomplete.
    /// </summary>
    public bool AllowSearch { get; set; } = true;

    /// <summary>
    /// Permite limpar a seleção.
    /// </summary>
    public bool AllowClear { get; set; } = true;

    /// <summary>
    /// Campo pai para lookup em cascata.
    /// </summary>
    public string DependsOn { get; set; } = string.Empty;

    /// <summary>
    /// Parâmetro de filtro enviado ao endpoint pai.
    /// </summary>
    public string DependsOnParam { get; set; } = string.Empty;

    /// <summary>
    /// Número mínimo de caracteres para iniciar busca.
    /// </summary>
    public int MinSearchLength { get; set; } = 0;

    /// <summary>
    /// Permite seleção múltipla.
    /// </summary>
    public bool Multiple { get; set; } = false;
}

// =============================================================================
// ATRIBUTOS DE UI - AÇÕES CUSTOMIZADAS
// =============================================================================

/// <summary>
/// Define uma ação customizada para a entidade.
/// Exemplo: "Aprovar", "Rejeitar", "Enviar Email"
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class EntityActionAttribute : Attribute
{
    /// <summary>
    /// Identificador único da ação.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Texto exibido no botão.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Ícone do botão (FontAwesome).
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// Classe CSS do botão.
    /// </summary>
    public string CssClass { get; set; } = "btn-secondary";

    /// <summary>
    /// Mensagem de confirmação.
    /// </summary>
    public string ConfirmMessage { get; set; } = string.Empty;

    /// <summary>
    /// Endpoint da API para executar a ação.
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Método HTTP: "POST", "PUT", "PATCH", "DELETE"
    /// </summary>
    public string HttpMethod { get; set; } = "POST";

    /// <summary>
    /// Exibir na listagem (por registro).
    /// </summary>
    public bool ShowInList { get; set; } = true;

    /// <summary>
    /// Exibir no formulário de detalhes/edição.
    /// </summary>
    public bool ShowInForm { get; set; } = true;

    /// <summary>
    /// Código de permissão necessária.
    /// </summary>
    public string RequiredPermission { get; set; } = string.Empty;

    public EntityActionAttribute(string name) => Name = name;
}

// =============================================================================
// ATRIBUTOS DE UI - FILTROS AVANÇADOS
// =============================================================================

/// <summary>
/// Configura um filtro avançado para a listagem.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class FilterConfigAttribute : Attribute
{
    /// <summary>
    /// Exibir no painel de filtros avançados.
    /// </summary>
    public bool Show { get; set; } = true;

    /// <summary>
    /// Tipo de filtro: "text", "select", "date", "daterange", "number", "numberrange", "boolean"
    /// </summary>
    public string FilterType { get; set; } = "text";

    /// <summary>
    /// Operador padrão: "equals", "contains", "startswith", "endswith", "gt", "gte", "lt", "lte", "between"
    /// </summary>
    public string DefaultOperator { get; set; } = "contains";

    /// <summary>
    /// Ordem no painel de filtros.
    /// </summary>
    public int Order { get; set; } = 0;

    /// <summary>
    /// Placeholder do campo de filtro.
    /// </summary>
    public string Placeholder { get; set; } = string.Empty;
}

// =============================================================================
// ATRIBUTOS DE INFRAESTRUTURA - BANCO DE DADOS
// =============================================================================

/// <summary>
/// Indica que a entidade possui triggers no banco de dados (auditoria, validações, etc).
/// Entidades marcadas com este atributo terão UseSqlOutputClause(false) aplicado
/// automaticamente no DbContext para evitar conflitos com triggers SQL Server.
/// 
/// PROBLEMA: SQL Server não permite OUTPUT INSERTED/DELETED em tabelas com triggers AFTER.
/// SOLUÇÃO: Entity Framework Core desabilita OUTPUT e faz SELECT separado após INSERT/UPDATE.
/// 
/// USO:
/// <code>
/// [HasDatabaseTriggers("Auditoria automática via triggers")]
/// public class MinhaEntidade
/// {
///     // ... propriedades
/// }
/// </code>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class HasDatabaseTriggersAttribute : Attribute
{
    /// <summary>
    /// Descrição dos triggers aplicados (opcional, para documentação).
    /// Exemplos: "Auditoria automática", "Validação de regras de negócio", "Logs de alteração"
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Lista de triggers conhecidos na tabela (opcional, para documentação).
    /// </summary>
    public string[] TriggerNames { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Construtor padrão.
    /// </summary>
    public HasDatabaseTriggersAttribute()
    {
    }

    /// <summary>
    /// Construtor com descrição.
    /// </summary>
    /// <param name="description">Descrição dos triggers</param>
    public HasDatabaseTriggersAttribute(string description)
    {
        Description = description;
    }

    /// <summary>
    /// Construtor com descrição e lista de triggers.
    /// </summary>
    /// <param name="description">Descrição dos triggers</param>
    /// <param name="triggerNames">Nomes dos triggers</param>
    public HasDatabaseTriggersAttribute(string description, params string[] triggerNames)
    {
        Description = description;
        TriggerNames = triggerNames;
    }
}