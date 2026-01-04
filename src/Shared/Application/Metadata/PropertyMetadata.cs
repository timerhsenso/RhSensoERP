// =============================================================================
// RHSENSOERP - PROPERTY METADATA
// =============================================================================
// Arquivo: src/Shared/Application/Metadata/PropertyMetadata.cs
// Descrição: Modelo de metadados de propriedade/campo para UI dinâmica
// =============================================================================

namespace RhSensoERP.Shared.Application.Metadata;

/// <summary>
/// Metadados completos de uma propriedade/campo da entidade.
/// </summary>
public class PropertyMetadata
{
    // =========================================================================
    // IDENTIFICAÇÃO
    // =========================================================================

    /// <summary>
    /// Nome da propriedade C#.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Nome amigável para exibição.
    /// </summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>
    /// Tipo C# da propriedade.
    /// </summary>
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// Nome da coluna no banco.
    /// </summary>
    public string? ColumnName { get; init; }

    // =========================================================================
    // VALIDAÇÃO
    // =========================================================================

    /// <summary>
    /// Campo é obrigatório.
    /// </summary>
    public bool IsRequired { get; init; }

    /// <summary>
    /// Campo aceita null.
    /// </summary>
    public bool IsNullable { get; init; }

    /// <summary>
    /// Comprimento máximo (strings).
    /// </summary>
    public int? MaxLength { get; init; }

    /// <summary>
    /// Comprimento mínimo (strings).
    /// </summary>
    public int? MinLength { get; init; }

    /// <summary>
    /// Valor mínimo (numéricos).
    /// </summary>
    public decimal? MinValue { get; init; }

    /// <summary>
    /// Valor máximo (numéricos).
    /// </summary>
    public decimal? MaxValue { get; init; }

    /// <summary>
    /// Padrão regex para validação.
    /// </summary>
    public string? RegexPattern { get; init; }

    /// <summary>
    /// Mensagem de erro de validação customizada.
    /// </summary>
    public string? ValidationMessage { get; init; }

    // =========================================================================
    // FLAGS
    // =========================================================================

    /// <summary>
    /// É chave primária.
    /// </summary>
    public bool IsPrimaryKey { get; init; }

    /// <summary>
    /// Campo somente leitura.
    /// </summary>
    public bool IsReadOnly { get; init; }

    /// <summary>
    /// Excluir do DTO.
    /// </summary>
    public bool ExcludeFromDto { get; init; }

    // =========================================================================
    // CONFIGURAÇÃO DE LISTAGEM
    // =========================================================================

    /// <summary>
    /// Configuração de exibição na listagem (DataTable).
    /// </summary>
    public ListPropertyConfig List { get; init; } = new();

    // =========================================================================
    // CONFIGURAÇÃO DE FORMULÁRIO
    // =========================================================================

    /// <summary>
    /// Configuração de exibição no formulário.
    /// </summary>
    public FormPropertyConfig Form { get; init; } = new();

    // =========================================================================
    // CONFIGURAÇÃO DE FILTRO
    // =========================================================================

    /// <summary>
    /// Configuração de filtro avançado.
    /// </summary>
    public FilterPropertyConfig Filter { get; init; } = new();

    // =========================================================================
    // LOOKUP (RELACIONAMENTO)
    // =========================================================================

    /// <summary>
    /// Configuração de lookup (se for relacionamento).
    /// </summary>
    public LookupConfig? Lookup { get; init; }
}

// =============================================================================
// CONFIGURAÇÃO DE LISTAGEM
// =============================================================================

/// <summary>
/// Configuração de exibição de propriedade na listagem.
/// </summary>
public class ListPropertyConfig
{
    /// <summary>
    /// Exibir na listagem.
    /// </summary>
    public bool Show { get; init; } = true;

    /// <summary>
    /// Ordem de exibição.
    /// </summary>
    public int Order { get; init; }

    /// <summary>
    /// Largura da coluna em pixels (null = auto).
    /// </summary>
    public int? Width { get; init; }

    /// <summary>
    /// Permite ordenação.
    /// </summary>
    public bool Sortable { get; init; } = true;

    /// <summary>
    /// Permite filtro.
    /// </summary>
    public bool Filterable { get; init; } = true;

    /// <summary>
    /// Formato de exibição.
    /// Valores: "text", "date", "datetime", "currency", "percent", "boolean", "badge"
    /// </summary>
    public string Format { get; init; } = "text";

    /// <summary>
    /// Classe CSS da coluna.
    /// </summary>
    public string? CssClass { get; init; }

    /// <summary>
    /// Alinhamento: "left", "center", "right"
    /// </summary>
    public string Align { get; init; } = "left";

    /// <summary>
    /// Template customizado (Handlebars/Mustache).
    /// </summary>
    public string? Template { get; init; }
}

// =============================================================================
// CONFIGURAÇÃO DE FORMULÁRIO
// =============================================================================

/// <summary>
/// Configuração de exibição de propriedade no formulário.
/// </summary>
public class FormPropertyConfig
{
    /// <summary>
    /// Exibir no formulário.
    /// </summary>
    public bool Show { get; init; } = true;

    /// <summary>
    /// Exibir no formulário de criação.
    /// </summary>
    public bool ShowOnCreate { get; init; } = true;

    /// <summary>
    /// Exibir no formulário de edição.
    /// </summary>
    public bool ShowOnEdit { get; init; } = true;

    /// <summary>
    /// Ordem de exibição.
    /// </summary>
    public int Order { get; init; }

    /// <summary>
    /// Grupo/seção do formulário.
    /// </summary>
    public string? Group { get; init; }

    /// <summary>
    /// Tipo de input HTML.
    /// Valores: "text", "textarea", "select", "date", "datetime", "time",
    /// "number", "email", "tel", "url", "password", "checkbox", "radio",
    /// "hidden", "file", "color", "range"
    /// </summary>
    public string InputType { get; init; } = "text";

    /// <summary>
    /// Texto placeholder.
    /// </summary>
    public string? Placeholder { get; init; }

    /// <summary>
    /// Texto de ajuda.
    /// </summary>
    public string? HelpText { get; init; }

    /// <summary>
    /// Máscara de input.
    /// </summary>
    public string? Mask { get; init; }

    /// <summary>
    /// Número de linhas (textarea).
    /// </summary>
    public int Rows { get; init; } = 3;

    /// <summary>
    /// Tamanho da coluna Bootstrap (1-12).
    /// </summary>
    public int ColSize { get; init; } = 6;

    /// <summary>
    /// Ícone do campo.
    /// </summary>
    public string? Icon { get; init; }

    /// <summary>
    /// Campo desabilitado.
    /// </summary>
    public bool Disabled { get; init; }

    /// <summary>
    /// Valor padrão.
    /// </summary>
    public string? DefaultValue { get; init; }

    /// <summary>
    /// Classe CSS adicional.
    /// </summary>
    public string? CssClass { get; init; }
}

// =============================================================================
// CONFIGURAÇÃO DE FILTRO
// =============================================================================

/// <summary>
/// Configuração de filtro avançado.
/// </summary>
public class FilterPropertyConfig
{
    /// <summary>
    /// Exibir no painel de filtros.
    /// </summary>
    public bool Show { get; init; } = true;

    /// <summary>
    /// Tipo de filtro.
    /// Valores: "text", "select", "date", "daterange", "number", "numberrange", "boolean"
    /// </summary>
    public string FilterType { get; init; } = "text";

    /// <summary>
    /// Operador padrão.
    /// Valores: "equals", "contains", "startswith", "endswith", "gt", "gte", "lt", "lte", "between"
    /// </summary>
    public string DefaultOperator { get; init; } = "contains";

    /// <summary>
    /// Ordem no painel.
    /// </summary>
    public int Order { get; init; }

    /// <summary>
    /// Placeholder.
    /// </summary>
    public string? Placeholder { get; init; }

    /// <summary>
    /// Opções pré-definidas (para select).
    /// </summary>
    public List<FilterOption>? Options { get; init; }
}

/// <summary>
/// Opção de filtro select.
/// </summary>
public class FilterOption
{
    /// <summary>
    /// Valor da opção.
    /// </summary>
    public string Value { get; init; } = string.Empty;

    /// <summary>
    /// Texto de exibição.
    /// </summary>
    public string Text { get; init; } = string.Empty;
}

// =============================================================================
// CONFIGURAÇÃO DE LOOKUP
// =============================================================================

/// <summary>
/// Configuração de lookup/relacionamento.
/// </summary>
public class LookupConfig
{
    /// <summary>
    /// Endpoint da API para buscar opções.
    /// </summary>
    public string Endpoint { get; init; } = string.Empty;

    /// <summary>
    /// Campo do valor (ID).
    /// </summary>
    public string ValueField { get; init; } = "Id";

    /// <summary>
    /// Campo do texto de exibição.
    /// </summary>
    public string TextField { get; init; } = "Nome";

    /// <summary>
    /// Permite busca/autocomplete.
    /// </summary>
    public bool AllowSearch { get; init; } = true;

    /// <summary>
    /// Permite limpar seleção.
    /// </summary>
    public bool AllowClear { get; init; } = true;

    /// <summary>
    /// Campo pai (cascata).
    /// </summary>
    public string? DependsOn { get; init; }

    /// <summary>
    /// Parâmetro de filtro do pai.
    /// </summary>
    public string? DependsOnParam { get; init; }

    /// <summary>
    /// Mínimo de caracteres para busca.
    /// </summary>
    public int MinSearchLength { get; init; } = 0;

    /// <summary>
    /// Permite seleção múltipla.
    /// </summary>
    public bool Multiple { get; init; } = false;

    /// <summary>
    /// Campos adicionais a retornar.
    /// </summary>
    public List<string>? AdditionalFields { get; init; }

    // <summary>
    /// Módulo da API (ex: "AdministracaoPessoal").
    /// Usado para lookups cross-module.
    /// </summary>
    public string? Module { get; init; }

    /// <summary>
    /// Route da entidade na API (ex: "tiposanguineo").
    /// </summary>
    public string? Route { get; init; }
}