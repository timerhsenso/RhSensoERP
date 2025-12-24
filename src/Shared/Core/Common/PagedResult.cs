namespace RhSensoERP.Shared.Core.Common;

/// <summary>
/// Representa um resultado paginado.
/// </summary>
/// <typeparam name="T">Tipo dos itens da lista.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PagedResult{T}"/> class.
    /// </summary>
    public PagedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    /// <summary>
    /// Gets os itens da página atual.
    /// </summary>
    public IEnumerable<T> Items { get; }

    /// <summary>
    /// Gets o total de registros.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Gets o número da página atual.
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets o tamanho da página.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets o total de páginas.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Gets a value indicating whether há página anterior.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Gets a value indicating whether há próxima página.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
