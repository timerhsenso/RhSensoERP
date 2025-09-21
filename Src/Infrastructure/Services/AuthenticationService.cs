using Microsoft.Extensions.Logging;
using RhSensoERP.Application.Auth;
using RhSensoERP.Application.Security.Auth.DTOs;
using RhSensoERP.Application.Security.Auth.Services;
using RhSensoERP.Core.Security.Auth;
using RhSensoERP.Infrastructure.Auth;

namespace RhSensoERP.Infrastructure.Services;

/// <summary>
/// Serviço de autenticação unificado
/// Orquestra estratégias de autenticação e carregamento de permissões
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthStrategyFactory _strategyFactory;
    private readonly ILegacyAuthService _legacyAuthService;
    private readonly JwtTokenService _jwtTokenService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IAuthStrategyFactory strategyFactory,
        ILegacyAuthService legacyAuthService,
        JwtTokenService jwtTokenService,
        ILogger<AuthenticationService> logger)
    {
        _strategyFactory = strategyFactory;
        _legacyAuthService = legacyAuthService;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    /// <summary>
    /// Autentica usando estratégia padrão configurada
    /// </summary>
    public async Task<AuthenticationResult> AuthenticateAsync(LoginRequest request, CancellationToken ct = default)
    {
        var strategy = _strategyFactory.GetDefault();
        return await AuthenticateWithStrategyAsync(strategy, request, ct);
    }

    /// <summary>
    /// Autentica usando estratégia específica
    /// </summary>
    public async Task<AuthenticationResult> AuthenticateAsync(LoginRequest request, AuthMode mode, CancellationToken ct = default)
    {
        var strategy = _strategyFactory.Get(mode);
        return await AuthenticateWithStrategyAsync(strategy, request, ct);
    }

    /// <summary>
    /// Executa autenticação com estratégia específica e pós-processamento
    /// </summary>
    private async Task<AuthenticationResult> AuthenticateWithStrategyAsync(
        IAuthStrategy strategy,
        LoginRequest request,
        CancellationToken ct)
    {
        try
        {
            _logger.LogDebug("Iniciando autenticação com estratégia {Strategy} para usuário {User}",
                strategy.Mode, request.UserOrEmail);

            // Executa autenticação na estratégia
            var strategyResult = await strategy.AuthenticateAsync(request, ct);

            if (!strategyResult.Success)
            {
                _logger.LogWarning("Falha na autenticação {Strategy}: {Error}",
                    strategy.Mode, strategyResult.ErrorMessage);

                return new AuthenticationResult(
                    Success: false,
                    ErrorMessage: strategyResult.ErrorMessage,
                    AccessToken: null,
                    UserData: null,
                    Provider: strategyResult.Provider);
            }

            // Sucesso na estratégia - carregar permissões e dados completos
            return await PostProcessSuccessfulAuthAsync(strategyResult, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante autenticação {Strategy}", strategy.Mode);

            return new AuthenticationResult(
                Success: false,
                ErrorMessage: "Erro interno do servidor",
                AccessToken: null,
                UserData: null,
                Provider: strategy.Mode.ToString());
        }
    }

    /// <summary>
    /// Pós-processamento após autenticação bem-sucedida
    /// Carrega permissões e dados adicionais do usuário
    /// </summary>
    private async Task<AuthenticationResult> PostProcessSuccessfulAuthAsync(
        AuthStrategyResult strategyResult,
        CancellationToken ct)
    {
        try
        {
            var userKey = strategyResult.UserKey!;

            // Para estratégias que já retornam dados completos (OnPrem)
            if (strategyResult.AdditionalData?.ContainsKey("UserData") == true)
            {
                var userData = (UserSessionData)strategyResult.AdditionalData["UserData"];
                var accessToken = (string)strategyResult.AdditionalData["AccessToken"];

                // Carregar permissões usando o serviço legacy
                var permissions = await _legacyAuthService.GetUserPermissionsAsync(userKey, ct);

                _logger.LogInformation("Autenticação completa bem-sucedida para {User} via {Provider}",
                    userKey, strategyResult.Provider);

                return new AuthenticationResult(
                    Success: true,
                    ErrorMessage: null,
                    AccessToken: accessToken,
                    UserData: userData,
                    Groups: permissions.Groups,
                    Permissions: permissions.Permissions,
                    Provider: strategyResult.Provider);
            }

            // Para futuras estratégias (SaaS, Windows) - gerar token próprio
            var tenantId = strategyResult.TenantId ?? Guid.NewGuid();
            var newToken = _jwtTokenService.CreateAccessToken(
                userKey,
                tenantId,
                new[] { "basic.access" }); // Permissões básicas por enquanto

            return new AuthenticationResult(
                Success: true,
                ErrorMessage: null,
                AccessToken: newToken,
                UserData: null, // Será implementado quando fizer SaaS/Windows
                Groups: new List<UserGroup>(),
                Permissions: new List<UserPermission>(),
                Provider: strategyResult.Provider);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no pós-processamento da autenticação para usuário {User}",
                strategyResult.UserKey);

            return new AuthenticationResult(
                Success: false,
                ErrorMessage: "Erro ao carregar dados do usuário",
                AccessToken: null,
                UserData: null,
                Provider: strategyResult.Provider);
        }
    }
}