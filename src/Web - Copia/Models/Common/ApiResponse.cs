// ============================================================================
// RHSENSOERP WEB - API RESPONSE
// ============================================================================
// Arquivo: src/Web/Models/Common/ApiResponse.cs
// Descrição: Modelos de resposta padronizada da API com mapeamento JSON correto
// ============================================================================

using System.Text.Json.Serialization;

namespace RhSensoERP.Web.Models.Common;

/// <summary>
/// Resposta padronizada da API.
/// Mapeia corretamente a estrutura Result{T} da API.
/// </summary>
/// <typeparam name="T">Tipo do dado retornado</typeparam>
public sealed class ApiResponse<T>
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida.
    /// </summary>
    [JsonPropertyName("isSuccess")]
    public bool Success { get; set; }

    /// <summary>
    /// Dados retornados (mapeado de "value" na API).
    /// </summary>
    [JsonPropertyName("value")]
    public T? Data { get; set; }

    /// <summary>
    /// Objeto de erro da API.
    /// </summary>
    [JsonPropertyName("error")]
    public ApiError? Error { get; set; }

    /// <summary>
    /// Mensagem de erro simplificada (para compatibilidade).
    /// </summary>
    public string? Message => Error?.Message;

    /// <summary>
    /// Erros de validação (se houver).
    /// </summary>
    [JsonPropertyName("validationErrors")]
    public Dictionary<string, List<string>>? Errors { get; set; }
}

/// <summary>
/// Objeto de erro retornado pela API.
/// </summary>
public sealed class ApiError
{
    /// <summary>
    /// Código do erro.
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>
    /// Mensagem de erro.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

/// <summary>
/// Resposta paginada da API.
/// Mapeia corretamente a estrutura PagedResult{T} da API.
/// </summary>
/// <typeparam name="T">Tipo do item da lista</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>
    /// Lista de itens da página atual.
    /// </summary>
    [JsonPropertyName("items")]
    public IEnumerable<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Número da página atual (mapeado de "pageNumber" na API).
    /// </summary>
    [JsonPropertyName("pageNumber")]
    public int Page { get; set; }

    /// <summary>
    /// Tamanho da página.
    /// </summary>
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    /// <summary>
    /// Total de registros.
    /// </summary>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    /// <summary>
    /// Total de páginas.
    /// </summary>
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }

    /// <summary>
    /// Indica se há página anterior.
    /// </summary>
    [JsonPropertyName("hasPreviousPage")]
    public bool HasPreviousPage { get; set; }

    /// <summary>
    /// Indica se há próxima página.
    /// </summary>
    [JsonPropertyName("hasNextPage")]
    public bool HasNextPage { get; set; }
}