// RhSensoERP.Identity/Domain/Entities/PasswordHistory.cs

namespace RhSensoERP.Identity.Core.Entities;

/// <summary>
/// Histórico de senhas anteriores para evitar reutilização.
/// </summary>
public class PasswordHistory
{
    public Guid Id { get; private set; }
    public Guid IdUserSecurity { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public string PasswordAlgorithm { get; private set; } = string.Empty;
    public DateTime ChangedAt { get; private set; }
    public string? ChangedByIP { get; private set; }
    public string? ChangeReason { get; private set; }

    // Navegação
    public virtual UserSecurity? UserSecurity { get; private set; }

    private PasswordHistory() { } // EF Core

    public PasswordHistory(
        Guid idUserSecurity,
        string passwordHash,
        string passwordAlgorithm,
        string? changeReason = null,
        string? changedByIP = null)
    {
        Id = Guid.NewGuid();
        IdUserSecurity = idUserSecurity;
        PasswordHash = passwordHash;
        PasswordAlgorithm = passwordAlgorithm;
        ChangeReason = changeReason;
        ChangedByIP = changedByIP;
        ChangedAt = DateTime.UtcNow;
    }
}