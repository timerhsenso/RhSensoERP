namespace RhSensoWeb.Models.Shared;

/// <summary>
/// Parâmetros de requisição do DataTables para paginação server-side
/// </summary>
public class DataTablesRequest
{
    /// <summary>
    /// Número da página (draw counter)
    /// </summary>
    public int Draw { get; set; }

    /// <summary>
    /// Índice do primeiro registro
    /// </summary>
    public int Start { get; set; }

    /// <summary>
    /// Número de registros por página
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Termo de busca global
    /// </summary>
    public string Search { get; set; } = string.Empty;

    /// <summary>
    /// Coluna para ordenação
    /// </summary>
    public string OrderColumn { get; set; } = string.Empty;

    /// <summary>
    /// Direção da ordenação (asc/desc)
    /// </summary>
    public string OrderDirection { get; set; } = "asc";

    /// <summary>
    /// Filtros específicos por coluna
    /// </summary>
    public Dictionary<string, string> ColumnFilters { get; set; } = new();

    /// <summary>
    /// Página atual (calculada)
    /// </summary>
    public int Page => Length > 0 ? (Start / Length) + 1 : 1;

    /// <summary>
    /// Tamanho da página
    /// </summary>
    public int PageSize => Length > 0 ? Length : 10;
}

/// <summary>
/// Resposta do DataTables para paginação server-side
/// </summary>
/// <typeparam name="T">Tipo dos dados da tabela</typeparam>
public class DataTablesResponse<T>
{
    /// <summary>
    /// Número da requisição (echo do draw)
    /// </summary>
    public int Draw { get; set; }

    /// <summary>
    /// Total de registros (sem filtro)
    /// </summary>
    public int RecordsTotal { get; set; }

    /// <summary>
    /// Total de registros filtrados
    /// </summary>
    public int RecordsFiltered { get; set; }

    /// <summary>
    /// Dados da página atual
    /// </summary>
    public List<T> Data { get; set; } = new();

    /// <summary>
    /// Mensagem de erro (opcional)
    /// </summary>
    public string? Error { get; set; }
}
