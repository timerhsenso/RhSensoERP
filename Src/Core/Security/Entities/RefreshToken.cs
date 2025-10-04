using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.Security.Entities;

/// <summary>
/// Entidade para armazenamento de Refresh Tokens
/// ✅ SEGURANÇA: Tokens são armazenados como hash SHA256
/// </summary>
[Table("RefreshTokens", Schema = "seg")]
public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Código do usuário (chave estrangeira para tuse1)
    /// </summary>
    [Required]
    [MaxLength(30)]
    public string CdUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Hash SHA256 do refresh token (NUNCA armazenar token em texto puro)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Data de expiração do refresh token (padrão: 7 dias)
    /// </summary>
    [Required]
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Data de criação do token
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Usuário que revogou o token (se revogado)
    /// </summary>
    [MaxLength(30)]
    public string? RevokedBy { get; set; }

    /// <summary>
    /// Data de revogação (se revogado)
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Token que substituiu este (rotation de refresh tokens)
    /// </summary>
    [MaxLength(100)]
    public string? ReplacedByToken { get; set; }

    /// <summary>
    /// Endereço IP que criou o token
    /// </summary>
    [Required]
    [MaxLength(45)]
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// User Agent do cliente que criou o token
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// JTI (ID único) do Access Token associado
    /// </summary>
    [MaxLength(50)]
    public string? AccessTokenJti { get; set; }

    // ========================================
    // PROPRIEDADES CALCULADAS
    // ========================================

    /// <summary>
    /// Verifica se o token está expirado
    /// </summary>
    [NotMapped]
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    /// <summary>
    /// Verifica se o token foi revogado
    /// </summary>
    [NotMapped]
    public bool IsRevoked => RevokedAt != null;

    /// <summary>
    /// Verifica se o token está ativo (não expirado e não revogado)
    /// </summary>
    [NotMapped]
    public bool IsActive => !IsRevoked && !IsExpired;
}