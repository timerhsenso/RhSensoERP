using RhSensoERP.Application.Security.Auth.DTOs;
using RhSensoERP.Shared.Core.Security.Auth;

namespace RhSensoERP.Shared.Application.Auth;

/// <summary>
/// Serviço de autenticação unificado
/// Orquestra as estratégias de autenticação e pós-processamento
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Autentica usuário usando a estratégia configurada
    /// </summary>
    /// <param name="request">Dados de login</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Resultado completo da autenticação com permissões</returns>
    Task<AuthenticationResult> AuthenticateAsync(LoginRequest request, CancellationToken ct = default);

    /// <summary>
    /// Autentica usuário usando estratégia específica
    /// </summary>
    /// <param name="request">Dados de login</param>
    /// <param name="mode">Modo de autenticação forçado</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Resultado completo da autenticação</returns>
    Task<AuthenticationResult> AuthenticateAsync(LoginRequest request, AuthMode mode, CancellationToken ct = default);
}

/// <summary>
/// Resultado completo da autenticação incluindo permissões
/// </summary>
public record AuthenticationResult(
    bool Success,
    string? ErrorMessage,
    string? AccessToken,
    UserSessionData? UserData,
    List<UserGroup>? Groups = null,
    List<UserPermission>? Permissions = null,
    string? Provider = null);