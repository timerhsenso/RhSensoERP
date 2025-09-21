using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RhSensoERP.Application.Auth;
using RhSensoERP.Application.Security.Auth.Services;
using RhSensoERP.Core.Security.Auth;

namespace RhSensoERP.Infrastructure.Auth.Strategies;

/// <summary>
/// Estratégia de autenticação OnPrem - usa o sistema legacy existente (tuse1)
/// Delega para ILegacyAuthService que já está funcionando
/// </summary>
public class OnPremAuthStrategy : IAuthStrategy
{
    private readonly ILegacyAuthService _legacyAuthService;
    private readonly AuthOptions _authOptions;
    private readonly ILogger<OnPremAuthStrategy> _logger;

    public AuthMode Mode => AuthMode.OnPrem;

    public OnPremAuthStrategy(
        ILegacyAuthService legacyAuthService,
        IOptionsMonitor<AuthOptions> authOptions,
        ILogger<OnPremAuthStrategy> logger)
    {
        _legacyAuthService = legacyAuthService;
        _authOptions = authOptions.CurrentValue;
        _logger = logger;
    }

    public async Task<AuthStrategyResult> AuthenticateAsync(LoginRequest request, CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("Iniciando autenticação OnPrem para usuário {Usuario}", request.UserOrEmail);

            // Validação básica
            if (string.IsNullOrWhiteSpace(request.UserOrEmail) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new AuthStrategyResult(
                    Success: false,
                    ErrorMessage: "Usuário e senha são obrigatórios",
                    UserKey: null,
                    DisplayName: null,
                    Provider: "OnPrem");
            }

            // Delega para o serviço legacy existente
            var legacyResult = await _legacyAuthService.AuthenticateAsync(
                request.UserOrEmail!,
                request.Password!,
                ct);

            if (!legacyResult.Success)
            {
                _logger.LogWarning("Falha na autenticação OnPrem para usuário {Usuario}: {Erro}",
                    request.UserOrEmail, legacyResult.ErrorMessage);

                return new AuthStrategyResult(
                    Success: false,
                    ErrorMessage: legacyResult.ErrorMessage ?? "Credenciais inválidas",
                    UserKey: null,
                    DisplayName: null,
                    Provider: "OnPrem");
            }

            // Sucesso - mapeia dados do resultado legacy
            var userData = legacyResult.UserData!;

            // Calcula TenantId baseado na empresa (como já faz no LegacyAuthService)
            var tenantId = userData.CdEmpresa.HasValue
                ? Guid.Parse($"{userData.CdEmpresa:D8}-0000-0000-0000-000000000000")
                : Guid.Parse("00000001-0000-0000-0000-000000000000");

            _logger.LogInformation("Autenticação OnPrem bem-sucedida para usuário {Usuario}", userData.CdUsuario);

            return new AuthStrategyResult(
                Success: true,
                ErrorMessage: null,
                UserKey: userData.CdUsuario,
                DisplayName: userData.DcUsuario,
                Provider: "OnPrem",
                TenantId: tenantId,
                AdditionalData: new Dictionary<string, object>
                {
                    ["UserData"] = userData,
                    ["AccessToken"] = legacyResult.AccessToken!,
                    ["NoUser"] = userData.NoUser,
                    ["TpUsuario"] = userData.TpUsuario,
                    ["CdEmpresa"] = userData.CdEmpresa,
                    ["CdFilial"] = userData.CdFilial
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante autenticação OnPrem para usuário {Usuario}", request.UserOrEmail);

            return new AuthStrategyResult(
                Success: false,
                ErrorMessage: "Erro interno do servidor",
                UserKey: null,
                DisplayName: null,
                Provider: "OnPrem");
        }
    }
}