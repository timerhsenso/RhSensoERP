using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RhSensoERP.Application.Auth;
using RhSensoERP.Core.Security.Auth;

namespace RhSensoERP.Infrastructure.Auth.Strategies;

/// <summary>
/// Estratégia de autenticação Windows - implementação futura
/// Por enquanto retorna "não implementado" para permitir testes com outras estratégias
/// </summary>
public class WindowsAuthStrategy : IAuthStrategy
{
    private readonly AuthOptions _authOptions;
    private readonly ILogger<WindowsAuthStrategy> _logger;

    public AuthMode Mode => AuthMode.Windows;

    public WindowsAuthStrategy(
        IOptionsMonitor<AuthOptions> authOptions,
        ILogger<WindowsAuthStrategy> logger)
    {
        _authOptions = authOptions.CurrentValue;
        _logger = logger;
    }

    public Task<AuthStrategyResult> AuthenticateAsync(LoginRequest request, CancellationToken ct = default)
    {
        _logger.LogWarning("Tentativa de usar estratégia Windows que ainda não foi implementada");

        var result = new AuthStrategyResult(
            Success: false,
            ErrorMessage: "Autenticação Windows não implementada ainda. Use modo OnPrem.",
            UserKey: null,
            DisplayName: null,
            Provider: "Windows");

        return Task.FromResult(result);
    }
}