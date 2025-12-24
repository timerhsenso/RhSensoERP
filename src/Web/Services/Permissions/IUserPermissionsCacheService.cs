// =============================================================================
// RHSENSOERP WEB - USER PERMISSIONS CACHE SERVICE INTERFACE
// =============================================================================
// Arquivo: src/Web/Services/Permissions/IUserPermissionsCacheService.cs
// Descrição: Interface para cache de permissões do usuário
// =============================================================================

using RhSensoERP.Web.Models.Account;

namespace RhSensoERP.Web.Services.Permissions;

/// <summary>
/// Service para gerenciamento de cache de permissões do usuário.
/// Usa IMemoryCache internamente e IAuthApiService como fallback.
/// </summary>
public interface IUserPermissionsCacheService
{
    /// <summary>
    /// Armazena permissões do usuário no cache.
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="permissions">ViewModel com as permissões</param>
    /// <param name="expiration">Tempo de expiração (default: 8 horas)</param>
    void Set(string cdUsuario, UserPermissionsViewModel permissions, TimeSpan? expiration = null);

    /// <summary>
    /// Obtém permissões do cache (sem fallback para API).
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <returns>Permissões ou null se não estiver em cache</returns>
    UserPermissionsViewModel? Get(string cdUsuario);

    /// <summary>
    /// Obtém permissões do cache ou busca na API se não existir.
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Permissões do usuário</returns>
    Task<UserPermissionsViewModel?> GetOrFetchAsync(string cdUsuario, CancellationToken ct = default);

    /// <summary>
    /// Obtém as ações permitidas (IAEC) para uma função específica.
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="cdFuncao">Código da função (ex: "RHU_FM_TAUX1")</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>String com ações permitidas (ex: "IAC" = Include, Alter, Consult)</returns>
    Task<string> GetPermissionsForFunctionAsync(string cdUsuario, string cdFuncao, CancellationToken ct = default);

    /// <summary>
    /// Verifica se o usuário tem permissão para uma ação específica em uma função.
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="cdFuncao">Código da função (ex: "RHU_FM_TAUX1")</param>
    /// <param name="acao">Ação: 'I' (Include), 'A' (Alter), 'E' (Exclude), 'C' (Consult)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>true se o usuário tem a permissão</returns>
    Task<bool> HasPermissionAsync(string cdUsuario, string cdFuncao, char acao, CancellationToken ct = default);

    /// <summary>
    /// Remove permissões do cache para um usuário.
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    void Remove(string cdUsuario);

    /// <summary>
    /// Atualiza (refresh) as permissões do usuário no cache.
    /// Remove o cache existente e busca novamente na API.
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Permissões atualizadas</returns>
    Task<UserPermissionsViewModel?> RefreshAsync(string cdUsuario, CancellationToken ct = default);
}