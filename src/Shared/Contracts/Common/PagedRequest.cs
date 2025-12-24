namespace RhSensoERP.Shared.Contracts.Common;

/// <summary>
/// Parâmetros genéricos de paginação, ordenação e busca.
/// </summary>
public sealed class PagedRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? SortBy { get; set; }
    public bool Desc { get; set; }

    /// <summary>Termo de busca opcional.</summary>
    public string? Search { get; set; }
}
