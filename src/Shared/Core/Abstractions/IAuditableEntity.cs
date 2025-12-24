namespace RhSensoERP.Shared.Core.Abstractions;

/// <summary>
/// Interface para entidades auditáveis.
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Data de criação.
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Usuário que criou.
    /// </summary>
    string? CreatedBy { get; set; }

    /// <summary>
    /// Data de atualização.
    /// </summary>
    DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Usuário que atualizou.
    /// </summary>
    string? UpdatedBy { get; set; }
}
