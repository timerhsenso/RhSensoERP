using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.Security.Entities;

/// <summary>
/// Entidade para usuários SaaS - completamente separada do sistema legacy
/// Implementa padrão de Domínio Rico com métodos de negócio intrínsecos
/// </summary>
public class SaasUser : BaseEntity
{
    // Autenticação
    public string Email { get; private set; } = string.Empty;
    public string EmailNormalized { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string PasswordSalt { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;

    // Controle de acesso
    public bool EmailConfirmed { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Tokens de segurança
    public string? EmailConfirmationToken { get; private set; }
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiry { get; private set; }

    // Refresh tokens
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }

    // Multi-tenant
    public Guid TenantId { get; private set; }

    // Auditoria e controle
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; private set; }
    public int LoginAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }

    // Metadados de sessão
    public string? CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }
    public string? UserAgent { get; private set; }
    public string? IpAddress { get; private set; }

    // Relacionamentos
    public virtual SaasTenant Tenant { get; set; } = null!;
    public virtual ICollection<SaasInvitation> SentInvitations { get; set; } = new List<SaasInvitation>();

    // Construtor protegido para EF Core
    protected SaasUser() { }

    // Propriedades de conveniência
    public string Username => Email;
    public string DisplayName => FullName;
    public string Name => FullName; // Compatibilidade com DTOs
    public string Salt => PasswordSalt; // Compatibilidade com AuthStrategy
    public bool IsLocked => LockedUntil.HasValue && LockedUntil > DateTime.UtcNow;
    public bool CanLogin => IsActive && EmailConfirmed && !IsLocked;

    // ========================================
    // MÉTODOS FACTORY (CRIAÇÃO)
    // ========================================

    /// <summary>
    /// Cria novo usuário SaaS
    /// </summary>
    /// <param name="fullName">Nome completo</param>
    /// <param name="email">Email</param>
    /// <param name="passwordHash">Hash da senha</param>
    /// <param name="passwordSalt">Salt da senha</param>
    /// <param name="tenantId">ID do tenant</param>
    /// <returns>Nova instância de SaasUser</returns>
    public static SaasUser Create(
        string fullName,
        string email,
        string passwordHash,
        string passwordSalt,
        Guid tenantId)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Nome é obrigatório", nameof(fullName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email é obrigatório", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Hash da senha é obrigatório", nameof(passwordHash));

        if (string.IsNullOrWhiteSpace(passwordSalt))
            throw new ArgumentException("Salt da senha é obrigatório", nameof(passwordSalt));

        if (tenantId == Guid.Empty)
            throw new ArgumentException("TenantId é obrigatório", nameof(tenantId));

        var user = new SaasUser
        {
            Id = Guid.NewGuid(),
            FullName = fullName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            EmailNormalized = email.Trim().ToUpperInvariant(),
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            TenantId = tenantId,
            EmailConfirmed = false, // Requer confirmação por padrão
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            LoginAttempts = 0
        };

        user.GenerateEmailConfirmationToken();
        return user;
    }

    // ========================================
    // MÉTODOS DE NEGÓCIO - AUTENTICAÇÃO
    // ========================================

    /// <summary>
    /// Registra login bem-sucedido
    /// </summary>
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        LoginAttempts = 0; // Reset em login bem-sucedido
        LockedUntil = null;
    }

    /// <summary>
    /// Incrementa tentativas de login
    /// </summary>
    public void IncrementLoginAttempts()
    {
        LoginAttempts++;
        UpdatedAt = DateTime.UtcNow;

        // Lockout após 5 tentativas
        if (LoginAttempts >= 5)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(15);
        }
    }

    /// <summary>
    /// Reset das tentativas de login
    /// </summary>
    public void ResetLoginAttempts()
    {
        LoginAttempts = 0;
        LockedUntil = null;
        UpdatedAt = DateTime.UtcNow;
    }

    // ========================================
    // MÉTODOS DE NEGÓCIO - EMAIL
    // ========================================

    /// <summary>
    /// Confirma email do usuário
    /// </summary>
    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        EmailConfirmationToken = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gera token de confirmação de email
    /// </summary>
    public void GenerateEmailConfirmationToken()
    {
        EmailConfirmationToken = Guid.NewGuid().ToString("N");
        UpdatedAt = DateTime.UtcNow;
    }

    // ========================================
    // MÉTODOS DE NEGÓCIO - SENHA
    // ========================================

    /// <summary>
    /// Atualiza senha do usuário
    /// </summary>
    /// <param name="newPasswordHash">Novo hash da senha</param>
    /// <param name="newPasswordSalt">Novo salt da senha</param>
    public void UpdatePassword(string newPasswordHash, string newPasswordSalt)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Hash da senha é obrigatório", nameof(newPasswordHash));

        if (string.IsNullOrWhiteSpace(newPasswordSalt))
            throw new ArgumentException("Salt da senha é obrigatório", nameof(newPasswordSalt));

        PasswordHash = newPasswordHash;
        PasswordSalt = newPasswordSalt;
        UpdatedAt = DateTime.UtcNow;

        // Limpa token de reset se existir
        ClearPasswordResetToken();

        // Reset tentativas de login
        ResetLoginAttempts();
    }

    /// <summary>
    /// Gera token de reset de senha
    /// </summary>
    public void GeneratePasswordResetToken()
    {
        PasswordResetToken = Guid.NewGuid().ToString("N");
        PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Valida se token de reset de senha é válido
    /// </summary>
    /// <param name="token">Token a validar</param>
    /// <returns>True se válido, false se inválido</returns>
    public bool IsPasswordResetTokenValid(string token)
    {
        return !string.IsNullOrEmpty(PasswordResetToken) &&
               PasswordResetToken == token &&
               PasswordResetTokenExpiry.HasValue &&
               PasswordResetTokenExpiry > DateTime.UtcNow;
    }

    /// <summary>
    /// Limpa token de reset de senha
    /// </summary>
    public void ClearPasswordResetToken()
    {
        PasswordResetToken = null;
        PasswordResetTokenExpiry = null;
        UpdatedAt = DateTime.UtcNow;
    }

    // ========================================
    // MÉTODOS DE NEGÓCIO - REFRESH TOKEN
    // ========================================

    /// <summary>
    /// Atualiza refresh token
    /// </summary>
    /// <param name="refreshToken">Novo refresh token</param>
    /// <param name="expiresAt">Data de expiração</param>
    public void UpdateRefreshToken(string refreshToken, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token é obrigatório", nameof(refreshToken));

        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Data de expiração deve ser no futuro", nameof(expiresAt));

        RefreshToken = refreshToken;
        RefreshTokenExpiresAt = expiresAt;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Valida se refresh token é válido
    /// </summary>
    /// <param name="token">Token a validar</param>
    /// <returns>True se válido, false se inválido</returns>
    public bool IsRefreshTokenValid(string token)
    {
        return !string.IsNullOrEmpty(RefreshToken) &&
               RefreshToken == token &&
               RefreshTokenExpiresAt.HasValue &&
               RefreshTokenExpiresAt > DateTime.UtcNow;
    }

    /// <summary>
    /// Limpa refresh token
    /// </summary>
    public void ClearRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiresAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    // ========================================
    // MÉTODOS DE NEGÓCIO - CONTROLE DE ACESSO
    // ========================================

    /// <summary>
    /// Ativa usuário
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Desativa usuário
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;

        // Limpa tokens ao desativar
        ClearRefreshToken();
        ClearPasswordResetToken();
    }

    /// <summary>
    /// Atualiza perfil do usuário
    /// </summary>
    /// <param name="fullName">Novo nome completo</param>
    public void UpdateProfile(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Nome é obrigatório", nameof(fullName));

        FullName = fullName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Atualiza metadados de sessão
    /// </summary>
    /// <param name="userAgent">User agent do navegador</param>
    /// <param name="ipAddress">Endereço IP</param>
    public void UpdateSessionMetadata(string? userAgent, string? ipAddress)
    {
        UserAgent = userAgent?.Trim();
        IpAddress = ipAddress?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}