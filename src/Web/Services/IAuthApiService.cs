// =============================================================================
// RHSENSOERP WEB - AUTH API SERVICE INTERFACE
// =============================================================================
// Arquivo: src/Web/Services/IAuthApiService.cs
// Descrição: Interface do serviço de autenticação via API
// Versão: 3.0 (Corrigido - Using correto do AuthResponse)
// =============================================================================

using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Web.Models.Account;

namespace RhSensoERP.Web.Services;

/// <summary>
/// Interface para serviço de autenticação via API REST.
/// </summary>
public interface IAuthApiService
{
    /// <summary>
    /// Realiza login do usuário na API.
    /// </summary>
    /// <param name="model">Dados de login</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Resposta de autenticação com tokens ou null se falhou</returns>
    Task<AuthResponse?> LoginAsync(LoginViewModel model, CancellationToken ct = default);

    /// <summary>
    /// Realiza logout do usuário na API.
    /// </summary>
    /// <param name="accessToken">Token de acesso JWT para autorização</param>
    /// <param name="refreshToken">Refresh token a ser invalidado</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>True se logout bem-sucedido</returns>
    Task<bool> LogoutAsync(string accessToken, string refreshToken, CancellationToken ct = default);

    /// <summary>
    /// Renova o access token usando o refresh token.
    /// </summary>
    /// <param name="refreshToken">Refresh token válido</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Nova resposta de autenticação ou null se falhou</returns>
    Task<AuthResponse?> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);

    /// <summary>
    /// Obtém informações do usuário atual.
    /// </summary>
    /// <param name="accessToken">Token de acesso JWT</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Informações do usuário ou null se falhou</returns>
    Task<UserInfoViewModel?> GetCurrentUserAsync(string accessToken, CancellationToken ct = default);

    /// <summary>
    /// Obtém permissões do usuário.
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="cdSistema">Código do sistema (opcional)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Permissões do usuário ou null se falhou</returns>
    Task<UserPermissionsViewModel?> GetUserPermissionsAsync(
        string cdUsuario, 
        string? cdSistema = null, 
        CancellationToken ct = default);
}
