// =============================================================================
// RHSENSOERP - SHARED APPLICATION
// =============================================================================
// Arquivo: src/Shared/RhSensoERP.Shared.Application/Interfaces/IService.cs
// Descrição: Interfaces de serviço de aplicação genérico
// =============================================================================

using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Shared.Application.Interfaces;

/// <summary>
/// Requisição paginada para serviços.
/// </summary>
public class ServicePagedRequest
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
    public static ServicePagedRequest Default => new();
}

/// <summary>
/// Resultado paginado de serviços.
/// </summary>
/// <typeparam name="T">Tipo dos itens</typeparam>
public class ServicePagedResult<T>
{
    /// <summary>
    /// Construtor padrão.
    /// </summary>
    public ServicePagedResult()
    {
        Items = new List<T>();
    }

    /// <summary>
    /// Construtor com parâmetros.
    /// </summary>
    public ServicePagedResult(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
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
    /// Cria um resultado vazio.
    /// </summary>
    public static ServicePagedResult<T> Empty(int pageNumber = 1, int pageSize = 10)
        => new(new List<T>(), 0, pageNumber, pageSize);

    /// <summary>
    /// Mapeia os itens para outro tipo.
    /// </summary>
    public ServicePagedResult<TDestination> Map<TDestination>(Func<T, TDestination> mapper)
    {
        return new ServicePagedResult<TDestination>(
            Items.Select(mapper).ToList(),
            TotalCount,
            PageNumber,
            PageSize);
    }
}

/// <summary>
/// Interface de serviço somente leitura.
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
/// <typeparam name="TDto">Tipo do DTO</typeparam>
public interface IReadService<TEntity, TKey, TDto>
    where TEntity : class
    where TKey : notnull
    where TDto : class
{
    /// <summary>
    /// Obtém um registro pelo ID.
    /// </summary>
    Task<TDto?> GetByIdAsync(TKey id, CancellationToken ct = default);

    /// <summary>
    /// Obtém todos os registros.
    /// </summary>
    Task<IReadOnlyList<TDto>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Obtém registros paginados.
    /// </summary>
    Task<ServicePagedResult<TDto>> GetPagedAsync(ServicePagedRequest request, CancellationToken ct = default);

    /// <summary>
    /// Verifica se existe um registro com o ID especificado.
    /// </summary>
    Task<bool> ExistsAsync(TKey id, CancellationToken ct = default);

    /// <summary>
    /// Conta o total de registros.
    /// </summary>
    Task<int> CountAsync(CancellationToken ct = default);
}

/// <summary>
/// Interface de serviço com operações de escrita.
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
/// <typeparam name="TDto">Tipo do DTO de leitura</typeparam>
/// <typeparam name="TCreateDto">Tipo do DTO de criação</typeparam>
/// <typeparam name="TUpdateDto">Tipo do DTO de atualização</typeparam>
public interface IWriteService<TEntity, TKey, TDto, TCreateDto, TUpdateDto>
    where TEntity : class
    where TKey : notnull
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    /// <summary>
    /// Cria um novo registro.
    /// </summary>
    Task<Result<TDto>> CreateAsync(TCreateDto createDto, CancellationToken ct = default);

    /// <summary>
    /// Atualiza um registro existente.
    /// </summary>
    Task<Result<TDto>> UpdateAsync(TKey id, TUpdateDto updateDto, CancellationToken ct = default);

    /// <summary>
    /// Remove um registro.
    /// </summary>
    Task<Result> DeleteAsync(TKey id, CancellationToken ct = default);

    /// <summary>
    /// Remove vários registros.
    /// </summary>
    Task<Result<int>> DeleteRangeAsync(IEnumerable<TKey> ids, CancellationToken ct = default);
}

/// <summary>
/// Interface completa de serviço CRUD.
/// </summary>
public interface ICrudService<TEntity, TKey, TDto, TCreateDto, TUpdateDto>
    : IReadService<TEntity, TKey, TDto>,
      IWriteService<TEntity, TKey, TDto, TCreateDto, TUpdateDto>
    where TEntity : class
    where TKey : notnull
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
}

/// <summary>
/// Interface simplificada de serviço CRUD (mesmo DTO para todas as operações).
/// </summary>
public interface ICrudService<TEntity, TKey, TDto>
    : ICrudService<TEntity, TKey, TDto, TDto, TDto>
    where TEntity : class
    where TKey : notnull
    where TDto : class
{
}

/// <summary>
/// Interface de mapper entre entidade e DTO.
/// </summary>
public interface IEntityMapper<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    /// <summary>
    /// Mapeia uma entidade para DTO.
    /// </summary>
    TDto ToDto(TEntity entity);

    /// <summary>
    /// Mapeia uma lista de entidades para DTOs.
    /// </summary>
    IReadOnlyList<TDto> ToDtoList(IEnumerable<TEntity> entities);

    /// <summary>
    /// Mapeia um DTO para entidade (criação).
    /// </summary>
    TEntity ToEntity(TDto dto);

    /// <summary>
    /// Atualiza uma entidade existente com dados do DTO.
    /// </summary>
    void UpdateEntity(TEntity entity, TDto dto);
}

/// <summary>
/// Interface de mapper com DTOs separados para criação e atualização.
/// </summary>
public interface IEntityMapper<TEntity, TDto, TCreateDto, TUpdateDto>
    where TEntity : class
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    /// <summary>
    /// Mapeia uma entidade para DTO de leitura.
    /// </summary>
    TDto ToDto(TEntity entity);

    /// <summary>
    /// Mapeia uma lista de entidades para DTOs de leitura.
    /// </summary>
    IReadOnlyList<TDto> ToDtoList(IEnumerable<TEntity> entities);

    /// <summary>
    /// Mapeia um DTO de criação para entidade.
    /// </summary>
    TEntity ToEntity(TCreateDto createDto);

    /// <summary>
    /// Atualiza uma entidade existente com dados do DTO de atualização.
    /// </summary>
    void UpdateEntity(TEntity entity, TUpdateDto updateDto);
}
