namespace RhSensoERP.Shared.Core.Security.Auth;

/// <summary>
/// Factory para criação de estratégias de autenticação
/// Resolve a estratégia correta baseada na configuração
/// </summary>
public interface IAuthStrategyFactory
{
    /// <summary>
    /// Obtém a estratégia de autenticação para o modo especificado
    /// </summary>
    /// <param name="mode">Modo de autenticação desejado</param>
    /// <returns>Instância da estratégia correspondente</returns>
    IAuthStrategy Get(AuthMode mode);

    /// <summary>
    /// Obtém a estratégia padrão baseada na configuração atual
    /// </summary>
    /// <returns>Estratégia configurada como padrão</returns>
    IAuthStrategy GetDefault();
}