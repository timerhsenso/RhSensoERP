using RhSensoWeb.Models.Auth;
using RhSensoWeb.Models.Shared;

namespace RhSensoWeb.Services.Interfaces;

/// <summary>
/// Interface para serviços de autenticação com a API
/// </summary>
public interface IAuthApiService
{
    /// <summary>
    /// Realiza login na API
    /// </summary>
    /// <param name="request">Dados de login</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resposta do login com token e dados do usuário</returns>
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida se o token JWT ainda é válido
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se o token é válido</returns>
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém informações atualizadas do usuário
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados atualizados do usuário</returns>
    Task<ApiResponse<UserDataDto>> GetUserInfoAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Realiza logout na API (invalidar token)
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado do logout</returns>
    Task<ApiResponse> LogoutAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renova o token JWT
    /// </summary>
    /// <param name="token">Token atual</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Novo token</returns>
    Task<ApiResponse<string>> RefreshTokenAsync(string token, CancellationToken cancellationToken = default);
}
