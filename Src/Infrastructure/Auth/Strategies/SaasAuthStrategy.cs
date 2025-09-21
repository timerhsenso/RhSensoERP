using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RhSensoERP.Application.Auth;
using RhSensoERP.Core.Security.Auth;

namespace RhSensoERP.Infrastructure.Auth.Strategies;

/// <summary>
/// Estratégia de autenticação SaaS - implementação futura
/// Por enquanto retorna "não implementado" para permitir testes com outras estratégias
/// </summary>
public class SaasAuthStrategy : IAuthStrategy
{
    private readonly AuthOptions _authOptions;
    private readonly ILogger<SaasAuthStrategy> _logger;

    public AuthMode Mode => AuthMode.SaaS;

    public SaasAuthStrategy(
        IOptionsMonitor<AuthOptions> authOptions,
        ILogger<SaasAuthStrategy> logger)
    {
        _authOptions = authOptions.CurrentValue;
        _logger = logger;
    }

    public Task<AuthStrategyResult> AuthenticateAsync(LoginRequest request, CancellationToken ct = default)
    {
        _logger.LogWarning("Tentativa de usar estratégia SaaS que ainda não foi implementada");

        var result = new AuthStrategyResult(
            Success: false,
            ErrorMessage: "Autenticação SaaS não implementada ainda. Use modo OnPrem.",
            UserKey: null,
            DisplayName: null,
            Provider: "SaaS");

        return Task.FromResult(result);
    }
}