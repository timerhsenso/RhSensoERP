// src/Web/Services/Base/IApiService.cs

using RhSensoERP.Web.Models.Common;

namespace RhSensoERP.Web.Services.Base;

/// <summary>
/// Interface genérica para serviços de comunicação com a API.
/// </summary>
/// <typeparam name="TDto">Tipo do DTO completo</typeparam>
/// <typeparam name="TCreateDto">Tipo do DTO de criação</typeparam>
/// <typeparam name="TUpdateDto">Tipo do DTO de atualização</typeparam>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
public interface IApiService<TDto, TCreateDto, TUpdateDto, TKey>
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    /// <summary>
    /// Obtém lista paginada.
    /// </summary>
    Task<ApiResponse<PagedResult<TDto>>> GetPagedAsync(int page, int pageSize, string? search = null);

    /// <summary>
    /// Obtém todos os registros (para combos/dropdowns).
    /// </summary>
    Task<ApiResponse<IEnumerable<TDto>>> GetAllAsync();

    /// <summary>
    /// Obtém registro por ID.
    /// </summary>
    Task<ApiResponse<TDto>> GetByIdAsync(TKey id);

    /// <summary>
    /// Cria novo registro.
    /// </summary>
    Task<ApiResponse<TDto>> CreateAsync(TCreateDto dto);

    /// <summary>
    /// Atualiza registro existente.
    /// </summary>
    Task<ApiResponse<TDto>> UpdateAsync(TKey id, TUpdateDto dto);

    /// <summary>
    /// Exclui registro.
    /// </summary>
    Task<ApiResponse<bool>> DeleteAsync(TKey id);

    /// <summary>
    /// Exclui múltiplos registros.
    /// </summary>
    Task<ApiResponse<bool>> DeleteMultipleAsync(IEnumerable<TKey> ids);
}
