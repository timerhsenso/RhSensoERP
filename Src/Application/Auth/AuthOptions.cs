using RhSensoERP.Core.Security.Auth;

namespace RhSensoERP.Application.Auth;

/// <summary>
/// Configurações de autenticação multi-estratégia
/// Mapeado do appsettings.json seção "Auth"
/// </summary>
public class AuthOptions
{
    /// <summary>
    /// Modo de autenticação ativo: SaaS, OnPrem ou Windows
    /// </summary>
    public AuthMode Mode { get; set; } = AuthMode.OnPrem;

    /// <summary>
    /// Configurações de senha e lockout
    /// </summary>
    public PasswordOptions Password { get; set; } = new();

    /// <summary>
    /// Configurações específicas do modo SaaS
    /// </summary>
    public SaasOptions SaaS { get; set; } = new();

    /// <summary>
    /// Configurações específicas do modo OnPrem
    /// </summary>
    public OnPremOptions OnPrem { get; set; } = new();

    /// <summary>
    /// Configurações específicas do Windows Authentication
    /// </summary>
    public WindowsOptions Windows { get; set; } = new();
}

/// <summary>
/// Configurações de senha e segurança
/// </summary>
public class PasswordOptions
{
    /// <summary>
    /// Algoritmo de hash: "BCrypt", "SHA256", etc.
    /// </summary>
    public string Hash { get; set; } = "BCrypt";

    /// <summary>
    /// Máximo de tentativas antes do lockout
    /// </summary>
    public int LockoutMaxFails { get; set; } = 5;

    /// <summary>
    /// Duração do lockout em minutos
    /// </summary>
    public int LockoutMinutes { get; set; } = 15;
}

/// <summary>
/// Configurações específicas do modo SaaS
/// </summary>
public class SaasOptions
{
    /// <summary>
    /// Requer confirmação de email para login
    /// </summary>
    public bool RequireEmailConfirmed { get; set; } = true;

    /// <summary>
    /// Permite auto-registro de usuários
    /// </summary>
    public bool AllowSelfRegistration { get; set; } = false;

    /// <summary>
    /// Duração do token de acesso em minutos
    /// </summary>
    public int TokenExpirationMinutes { get; set; } = 60;
}

/// <summary>
/// Configurações específicas do modo OnPrem
/// </summary>
public class OnPremOptions
{
    /// <summary>
    /// Nome da tabela de usuários (padrão: tuse1)
    /// </summary>
    public string UserTable { get; set; } = "tuse1";

    /// <summary>
    /// Permite reset de senha via interface
    /// </summary>
    public bool AllowPasswordReset { get; set; } = false;

    /// <summary>
    /// Requer que usuário seja do domínio
    /// </summary>
    public bool RequireDomainUser { get; set; } = false;
}

/// <summary>
/// Configurações específicas do Windows Authentication
/// </summary>
public class WindowsOptions
{
    /// <summary>
    /// Requer que usuário seja membro do domínio
    /// </summary>
    public bool RequireDomainMembership { get; set; } = true;

    /// <summary>
    /// Permite fallback para autenticação local se Windows falhar
    /// </summary>
    public bool FallbackToLocal { get; set; } = false;

    /// <summary>
    /// Nível de confiança: "Full", "Partial"
    /// </summary>
    public string TrustLevel { get; set; } = "Full";

    /// <summary>
    /// Domínio padrão quando não especificado pelo usuário
    /// </summary>
    public string DefaultDomain { get; set; } = Environment.UserDomainName;

    /// <summary>
    /// Endereço do Domain Controller principal
    /// </summary>
    public string? DomainController { get; set; }

    /// <summary>
    /// Lista de domínios permitidos para autenticação
    /// </summary>
    public List<string> AllowedDomains { get; set; } = new();

    /// <summary>
    /// Caminho LDAP para consultas no Active Directory
    /// </summary>
    public string? LdapPath { get; set; }

    /// <summary>
    /// Base DN para pesquisas LDAP
    /// </summary>
    public string? SearchBase { get; set; }

    /// <summary>
    /// Timeout para operações de autenticação Windows (em segundos)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Habilita cache de credenciais validadas (em minutos)
    /// </summary>
    public int CacheValidCredentialsMinutes { get; set; } = 5;
}