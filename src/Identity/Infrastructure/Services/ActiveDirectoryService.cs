// ============================================================================
// FASE 3 - Active Directory
// src/Identity/Infrastructure/Services/ActiveDirectoryService.cs
// ============================================================================

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RhSensoERP.Identity.Application.Services;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace RhSensoERP.Identity.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de autenticação Active Directory.
/// </summary>
public sealed class ActiveDirectoryService : IActiveDirectoryService
{
    private readonly ActiveDirectorySettings _settings;
    private readonly ILogger<ActiveDirectoryService> _logger;

    public ActiveDirectoryService(
        IOptions<ActiveDirectorySettings> settings,
        ILogger<ActiveDirectoryService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public bool IsAvailable()
    {
        return _settings.Enabled &&
               !string.IsNullOrWhiteSpace(_settings.Domain) &&
               !string.IsNullOrWhiteSpace(_settings.LdapPath);
    }

    public async Task<bool> AuthenticateAsync(
        string username,
        string password,
        string? domain = null,
        CancellationToken ct = default)
    {
        if (!IsAvailable())
        {
            _logger.LogWarning("🔴 AD: Serviço não está configurado ou disponível");
            return false;
        }

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("🔴 AD: Username ou password vazio");
            return false;
        }

        var domainToUse = domain ?? _settings.Domain;

        try
        {
            _logger.LogInformation("🔐 AD: Tentando autenticar {Username} no domínio {Domain}",
                username, domainToUse);

            // Executar em Task para permitir CancellationToken
            return await Task.Run(() =>
            {
                try
                {
                    using var context = new PrincipalContext(
                        ContextType.Domain,
                        domainToUse,
                        _settings.Container,
                        _settings.ServiceUsername,
                        _settings.ServicePassword);

                    // Validar credenciais
                    var isValid = context.ValidateCredentials(
                        username,
                        password,
                        ContextOptions.Negotiate);

                    if (isValid)
                    {
                        _logger.LogInformation("✅ AD: Autenticação bem-sucedida para {Username}", username);
                    }
                    else
                    {
                        _logger.LogWarning("❌ AD: Credenciais inválidas para {Username}", username);
                    }

                    return isValid;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ AD: Erro ao validar credenciais para {Username}", username);
                    return false;
                }
            }, ct);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("⏱️ AD: Autenticação cancelada para {Username}", username);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ AD: Erro inesperado ao autenticar {Username}", username);
            return false;
        }
    }

    public async Task<ActiveDirectoryUserInfo?> GetUserInfoAsync(
        string username,
        CancellationToken ct = default)
    {
        if (!IsAvailable())
        {
            _logger.LogWarning("🔴 AD: Serviço não está configurado");
            return null;
        }

        try
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var context = new PrincipalContext(
                        ContextType.Domain,
                        _settings.Domain,
                        _settings.Container,
                        _settings.ServiceUsername,
                        _settings.ServicePassword);

                    using var user = UserPrincipal.FindByIdentity(context, username);

                    if (user == null)
                    {
                        _logger.LogWarning("⚠️ AD: Usuário {Username} não encontrado", username);
                        return null;
                    }

                    var userInfo = new ActiveDirectoryUserInfo
                    {
                        Username = user.SamAccountName ?? username,
                        DisplayName = user.DisplayName,
                        Email = user.EmailAddress,
                        IsActive = user.Enabled ?? false
                    };

                    // Tentar obter informações adicionais via DirectoryEntry
                    if (user.GetUnderlyingObject() is DirectoryEntry entry)
                    {
                        userInfo.Department = entry.Properties["department"]?.Value?.ToString();
                        userInfo.Title = entry.Properties["title"]?.Value?.ToString();
                    }

                    _logger.LogInformation("✅ AD: Informações obtidas para {Username}", username);
                    return userInfo;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ AD: Erro ao obter informações de {Username}", username);
                    return null;
                }
            }, ct);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("⏱️ AD: Operação cancelada para {Username}", username);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ AD: Erro inesperado ao obter info de {Username}", username);
            return null;
        }
    }
}

/// <summary>
/// Configurações do Active Directory.
/// </summary>
public sealed class ActiveDirectorySettings
{
    public const string SectionName = "ActiveDirectory";

    /// <summary>
    /// Habilita autenticação via AD.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Domínio do AD (ex: "EMPRESA.LOCAL").
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// Caminho LDAP (ex: "LDAP://DC=empresa,DC=local").
    /// </summary>
    public string LdapPath { get; set; } = string.Empty;

    /// <summary>
    /// Container/OU onde os usuários estão (opcional).
    /// Ex: "OU=Users,DC=empresa,DC=local"
    /// </summary>
    public string? Container { get; set; }

    /// <summary>
    /// Usuário de serviço para consultas no AD (opcional).
    /// Se não informado, usa credenciais do pool de aplicação.
    /// </summary>
    public string? ServiceUsername { get; set; }

    /// <summary>
    /// Senha do usuário de serviço (opcional).
    /// </summary>
    public string? ServicePassword { get; set; }

    /// <summary>
    /// Timeout em segundos para operações no AD.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
}
