// =============================================================================
// RHSENSOERP - SHARED CONTRACTS
// =============================================================================
// Arquivo: src/Shared/RhSensoERP.Shared.Contracts/DTOs/PaginationDtos.cs
// Descrição: DTOs para paginação e resultados padronizados
// NOTA: Complementa ApiResponse.cs existente
// =============================================================================

namespace RhSensoERP.Shared.Contracts.DTOs;

/// <summary>
/// Interface marcadora para DTOs.
/// </summary>
public interface IDto
{
}

/// <summary>
/// Interface para DTOs com ID.
/// </summary>
/// <typeparam name="TKey">Tipo da chave</typeparam>
public interface IDto<TKey> : IDto
{
    /// <summary>
    /// ID do registro.
    /// </summary>
    TKey Id { get; set; }
}

/// <summary>
/// Requisição paginada.
/// </summary>
public class PagedRequest
{
    private int _pageNumber = 1;
    private int _pageSize = 10;

    /// <summary>
    /// Número da página (1-based).
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Tamanho da página.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 10 : (value > 100 ? 100 : value);
    }

    /// <summary>
    /// Campo para ordenação.
    /// </summary>
    public string? SortField { get; set; }

    /// <summary>
    /// Direção da ordenação (asc/desc).
    /// </summary>
    public string SortDirection { get; set; } = "asc";

    /// <summary>
    /// Termo de busca global.
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filtros adicionais (chave=valor).
    /// </summary>
    public Dictionary<string, string>? Filters { get; set; }

    /// <summary>
    /// Indica se a ordenação é descendente.
    /// </summary>
    public bool IsDescending => SortDirection?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;

    /// <summary>
    /// Calcula o número de registros a pular.
    /// </summary>
    public int Skip => (PageNumber - 1) * PageSize;

    /// <summary>
    /// Requisição padrão (página 1, 10 itens).
    /// </summary>
    public static PagedRequest Default => new();

    /// <summary>
    /// Requisição para obter todos (primeira página, 1000 itens).
    /// </summary>
    public static PagedRequest All => new() { PageSize = 1000 };
}

/// <summary>
/// Resultado paginado.
/// </summary>
/// <typeparam name="T">Tipo dos itens</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Construtor padrão.
    /// </summary>
    public PagedResult()
    {
        Items = new List<T>();
    }

    /// <summary>
    /// Construtor com parâmetros.
    /// </summary>
    public PagedResult(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    /// <summary>
    /// Lista de itens da página atual.
    /// </summary>
    public IReadOnlyList<T> Items { get; set; }

    /// <summary>
    /// Número da página atual.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Tamanho da página.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de registros.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total de páginas.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    /// <summary>
    /// Indica se há página anterior.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indica se há próxima página.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Índice do primeiro item na página.
    /// </summary>
    public int FirstItemIndex => TotalCount == 0 ? 0 : (PageNumber - 1) * PageSize + 1;

    /// <summary>
    /// Índice do último item na página.
    /// </summary>
    public int LastItemIndex => Math.Min(PageNumber * PageSize, TotalCount);

    /// <summary>
    /// Cria um resultado vazio.
    /// </summary>
    public static PagedResult<T> Empty(int pageNumber = 1, int pageSize = 10)
        => new(new List<T>(), 0, pageNumber, pageSize);

    /// <summary>
    /// Mapeia os itens para outro tipo.
    /// </summary>
    public PagedResult<TDestination> Map<TDestination>(Func<T, TDestination> mapper)
    {
        return new PagedResult<TDestination>(
            Items.Select(mapper).ToList(),
            TotalCount,
            PageNumber,
            PageSize);
    }
}

/// <summary>
/// Extensões para ApiResponse (complementa ApiResponse existente).
/// </summary>
public static class ApiResponseExtensions
{
    /// <summary>
    /// Cria uma resposta de sucesso.
    /// </summary>
    public static Common.ApiResponse<T> Ok<T>(T data, string? message = null)
    {
        return new Common.ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    /// <summary>
    /// Cria uma resposta de erro.
    /// </summary>
    public static Common.ApiResponse<T> Error<T>(string message)
    {
        return new Common.ApiResponse<T>
        {
            Success = false,
            Message = message
        };
    }

    /// <summary>
    /// Cria uma resposta de não encontrado.
    /// </summary>
    public static Common.ApiResponse<T> NotFound<T>(string? message = null)
    {
        return new Common.ApiResponse<T>
        {
            Success = false,
            Message = message ?? "Registro não encontrado"
        };
    }

    /// <summary>
    /// Cria uma resposta de erro de validação.
    /// </summary>
    public static Common.ApiResponse<T> ValidationError<T>(string message)
    {
        return new Common.ApiResponse<T>
        {
            Success = false,
            Message = message
        };
    }
}

/// <summary>
/// Resposta da API com suporte a erros detalhados.
/// </summary>
/// <typeparam name="T">Tipo dos dados</typeparam>
public class ApiResponseDetailed<T>
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Dados retornados.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Mensagem de retorno.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Lista de erros.
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Erros de validação por campo.
    /// </summary>
    public Dictionary<string, List<string>>? ValidationErrors { get; set; }

    /// <summary>
    /// Timestamp da resposta.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Cria resposta de sucesso.
    /// </summary>
    public static ApiResponseDetailed<T> Ok(T data, string? message = null)
    {
        return new ApiResponseDetailed<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    /// <summary>
    /// Cria resposta de erro.
    /// </summary>
    public static ApiResponseDetailed<T> Fail(string message)
    {
        return new ApiResponseDetailed<T>
        {
            Success = false,
            Message = message,
            Errors = new List<string> { message }
        };
    }

    /// <summary>
    /// Cria resposta de erro com múltiplos erros.
    /// </summary>
    public static ApiResponseDetailed<T> Fail(IEnumerable<string> errors)
    {
        var errorList = errors.ToList();
        return new ApiResponseDetailed<T>
        {
            Success = false,
            Message = errorList.FirstOrDefault(),
            Errors = errorList
        };
    }

    /// <summary>
    /// Cria resposta de erro de validação.
    /// </summary>
    public static ApiResponseDetailed<T> ValidationFail(Dictionary<string, List<string>> validationErrors)
    {
        return new ApiResponseDetailed<T>
        {
            Success = false,
            Message = "Erro de validação",
            ValidationErrors = validationErrors,
            Errors = validationErrors.SelectMany(v => v.Value).ToList()
        };
    }
}
