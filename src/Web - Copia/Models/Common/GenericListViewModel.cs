// =============================================================================
// RHSENSOERP.WEB - GENERIC LIST VIEW MODEL
// =============================================================================
// Arquivo: src/RhSensoERP.Web/Models/Common/GenericListViewModel.cs
// Descrição: ViewModel para a View parcial genérica de listagem
// =============================================================================

using RhSensoERP.Shared.Application.Metadata;

namespace RhSensoERP.Web.Models.Common;

/// <summary>
/// ViewModel para a View parcial genérica de listagem.
/// </summary>
public class GenericListViewModel
{
    /// <summary>
    /// Nome da entidade (ex: "Banco", "Sistema").
    /// </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// Metadados da entidade (carregados via MetadataService).
    /// </summary>
    public EntityMetadata? Metadata { get; set; }

    /// <summary>
    /// Exibir filtros avançados.
    /// </summary>
    public bool ShowAdvancedFilters { get; set; } = false;

    /// <summary>
    /// Título customizado da página (opcional).
    /// </summary>
    public string? CustomTitle { get; set; }

    /// <summary>
    /// URL base da API (opcional, usa do metadata se não informado).
    /// </summary>
    public string? ApiBaseUrl { get; set; }

    /// <summary>
    /// Parâmetros adicionais para a query da API.
    /// </summary>
    public Dictionary<string, string>? QueryParameters { get; set; }
}