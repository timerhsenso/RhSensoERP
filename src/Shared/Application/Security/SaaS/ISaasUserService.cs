using RhSensoERP.Shared.Core.Common;
using RhSensoERP.Shared.Core.Shared;

namespace RhSensoERP.Shared.Application.Security.SaaS;

/// <summary>
/// Interface para serviços de autenticação SaaS
/// </summary>
public interface ISaasUserService
{
    /// <summary>
    /// Autentica um usuário SaaS
    /// </summary>
    /// <param name="request">Dados de autenticação</param>
    /// <returns>Resultado da autenticação com token JWT</returns>
    Task<Result<SaasAuthenticationResponse>> AuthenticateAsync(SaasAuthenticationRequest request);

    /// <summary>
    /// Registra um novo usuário SaaS
    /// </summary>
    /// <param name="request">Dados de registro</param>
    /// <returns>Resultado do registro</returns>
    Task<ServiceResult<SaasRegistrationResponse>> RegisterAsync(SaasRegistrationRequest request);

    /// <summary>
    /// Renova token de acesso usando refresh token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <returns>Novo token de acesso</returns>
    Task<Result<SaasAuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequest request);

    Task<ServiceResult<ConfirmEmailResponse>> ConfirmEmailAsync(ConfirmEmailRequest request);
    Task<ServiceResult<object>> ResendConfirmationEmailAsync(ResendConfirmationRequest request);
}

/// <summary>
/// Request para autenticação SaaS
/// </summary>
public record SaasAuthenticationRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

/// <summary>
/// Response da autenticação SaaS
/// </summary>
public record SaasAuthenticationResponse
{
    public required string Token { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public required SaasUserResponse User { get; init; }
}

/// <summary>
/// Request para registro de usuário SaaS
/// </summary>
public record SaasRegistrationRequest
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required Guid TenantId { get; init; }
}

/// <summary>
/// Request para refresh token
/// </summary>
public record RefreshTokenRequest
{
    public required string RefreshToken { get; init; }
}

/// <summary>
/// Response com dados do usuário SaaS
/// </summary>
public record SaasUserResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required Guid TenantId { get; init; }
}
