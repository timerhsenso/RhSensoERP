// =============================================================================
// IAPI SERVICE - INTERFACE CORRIGIDA COM ORDENAÇÃO FUNCIONAL
// =============================================================================
// Versão: 4.0 FINAL
// Atualizado: 2025-12-30
// Changelog v4.0:
//   ✅ CORRIGIDO: orderBy → sortBy
//   ✅ CORRIGIDO: ascending → desc
//   ✅ Parâmetros compatíveis com PagedRequest do backend
// =============================================================================

using RhSensoERP.Web.Models.Common;

namespace RhSensoERP.Web.Services.Base;

/// <summary>
/// Interface padrão para serviços de API com suporte a CRUD e paginação.
/// v4.0: Corrigida para usar sortBy e desc (compatível com backend).
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
    /// Busca registros paginados com ordenação.
    /// ✅ v4.0: Parâmetros corretos (sortBy, desc).
    /// </summary>
    /// <param name="page">Número da página (iniciando em 1)</param>
    /// <param name="pageSize">Quantidade de registros por página</param>
    /// <param name="search">Termo de busca (opcional)</param>
    /// <param name="sortBy">Campo para ordenação (PascalCase) - ex: "Nome", "CodigoNr"</param>
    /// <param name="desc">true para descendente (DESC), false para ascendente (ASC)</param>
    Task<ApiResponse<PagedResult<TDto>>> GetPagedAsync(
        int page,
        int pageSize,
        string? search = null,
        string? sortBy = null,    // ✅ CORRETO: sortBy (não orderBy)
        bool desc = false);       // ✅ CORRETO: desc (não ascending)

    /// <summary>
    /// Busca todos os registros (sem paginação).
    /// </summary>
    Task<ApiResponse<IEnumerable<TDto>>> GetAllAsync();

    /// <summary>
    /// Busca um registro por ID.
    /// </summary>
    Task<ApiResponse<TDto>> GetByIdAsync(TKey id);

    /// <summary>
    /// Cria um novo registro.
    /// </summary>
    Task<ApiResponse<TDto>> CreateAsync(TCreateDto dto);

    /// <summary>
    /// Atualiza um registro existente.
    /// </summary>
    Task<ApiResponse<TDto>> UpdateAsync(TKey id, TUpdateDto dto);

    /// <summary>
    /// Exclui um registro por ID.
    /// </summary>
    Task<ApiResponse<bool>> DeleteAsync(TKey id);

    /// <summary>
    /// Exclui múltiplos registros.
    /// </summary>
    Task<ApiResponse<bool>> DeleteMultipleAsync(IEnumerable<TKey> ids);
}