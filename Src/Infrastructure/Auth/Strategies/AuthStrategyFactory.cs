using Microsoft.Extensions.Options;
using RhSensoERP.Application.Auth;
using RhSensoERP.Core.Security.Auth;

namespace RhSensoERP.Infrastructure.Auth.Strategies;

/// <summary>
/// Factory para resolução de estratégias de autenticação
/// Usa DI para resolver todas as estratégias disponíveis e seleciona baseado na configuração
/// </summary>
public class AuthStrategyFactory : IAuthStrategyFactory
{
    private readonly IEnumerable<IAuthStrategy> _strategies;
    private readonly AuthOptions _authOptions;

    public AuthStrategyFactory(IEnumerable<IAuthStrategy> strategies, IOptionsMonitor<AuthOptions> authOptions)
    {
        _strategies = strategies;
        _authOptions = authOptions.CurrentValue;
    }

    /// <summary>
    /// Obtém estratégia específica por modo
    /// </summary>
    public IAuthStrategy Get(AuthMode mode)
    {
        var strategy = _strategies.FirstOrDefault(s => s.Mode == mode);

        if (strategy == null)
        {
            throw new InvalidOperationException(
                $"Estratégia de autenticação '{mode}' não encontrada. " +
                $"Estratégias disponíveis: {string.Join(", ", _strategies.Select(s => s.Mode))}");
        }

        return strategy;
    }

    /// <summary>
    /// Obtém estratégia padrão baseada na configuração
    /// </summary>
    public IAuthStrategy GetDefault()
    {
        return Get(_authOptions.Mode);
    }
}