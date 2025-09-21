using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RhSensoERP.Application.Auth;
using RhSensoERP.Core.Security.Auth;

namespace RhSensoERP.Infrastructure.Auth.Strategies;

/// <summary>
/// Estratégia de autenticação Windows - implementação simplificada
/// Por enquanto aceita credenciais mas não valida via AD (para evitar dependências complexas)
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
        try
        {
            _logger.LogDebug("Iniciando autenticação Windows para usuário {Usuario} no domínio {Domain}",
                request.UserOrEmail, request.Domain);

            // Validação básica
            if (string.IsNullOrWhiteSpace(request.UserOrEmail) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Task.FromResult(new AuthStrategyResult(
                    Success: false,
                    ErrorMessage: "Usuário e senha são obrigatórios para autenticação Windows",
                    UserKey: null,
                    DisplayName: null,
                    Provider: "Windows"));
            }

            // Se não foi fornecido domínio, tenta usar o domínio da máquina atual
            var domain = request.Domain ?? Environment.UserDomainName;
            var username = request.UserOrEmail!;

            _logger.LogDebug("Processando autenticação {Usuario} no domínio {Domain}", username, domain);

            // IMPLEMENTAÇÃO SIMPLIFICADA: aceita qualquer credencial que siga o padrão
            // Em produção, implementar validação real via Active Directory
            var isValid = ValidateBasicWindowsFormat(domain, username, request.Password!);

            if (!isValid)
            {
                _logger.LogWarning("Formato inválido para autenticação Windows: {Domain}\\{Usuario}", domain, username);

                return Task.FromResult(new AuthStrategyResult(
                    Success: false,
                    ErrorMessage: "Formato de credenciais Windows inválido",
                    UserKey: null,
                    DisplayName: null,
                    Provider: "Windows"));
            }

            // Sucesso - retorna dados mockados para teste
            var userKey = $"{domain}\\{username}";
            var displayName = $"{username} ({domain})";

            _logger.LogInformation("Autenticação Windows simulada bem-sucedida para {UserKey}", userKey);

            return Task.FromResult(new AuthStrategyResult(
                Success: true,
                ErrorMessage: null,
                UserKey: userKey,
                DisplayName: displayName,
                Provider: "Windows",
                TenantId: Guid.Parse("10000000-0000-0000-0000-000000000000"), // Tenant padrão para Windows
                AdditionalData: new Dictionary<string, object>
                {
                    ["Domain"] = domain,
                    ["Username"] = username,
                    ["AuthMethod"] = "Windows",
                    ["Note"] = "Implementação simplificada - não validada via AD"
                }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante autenticação Windows para usuário {Usuario}", request.UserOrEmail);

            return Task.FromResult(new AuthStrategyResult(
                Success: false,
                ErrorMessage: "Erro interno durante autenticação Windows",
                UserKey: null,
                DisplayName: null,
                Provider: "Windows"));
        }
    }

    /// <summary>
    /// Validação básica de formato (não valida credenciais reais)
    /// </summary>
    private bool ValidateBasicWindowsFormat(string domain, string username, string password)
    {
        // Validações básicas de formato
        if (string.IsNullOrWhiteSpace(domain) || domain.Length < 2)
            return false;

        if (string.IsNullOrWhiteSpace(username) || username.Length < 2)
            return false;

        if (string.IsNullOrWhiteSpace(password) || password.Length < 3)
            return false;

        // Para teste: aceita se senha tem mais de 3 caracteres
        return password.Length >= 3;
    }
}