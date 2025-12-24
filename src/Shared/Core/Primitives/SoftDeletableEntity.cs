namespace RhSensoERP.Shared.Core.Primitives;

using RhSensoERP.Shared.Core.Abstractions;

/// <summary>
/// Entidade base com suporte a soft delete.
/// Herda auditoria de BaseEntity e adiciona soft delete.
/// </summary>
public abstract class SoftDeletableEntity : BaseEntity, ISoftDelete
{
    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <inheritdoc />
    public DateTime? DeletedAt { get; set; }

    /// <inheritdoc />
    public string? DeletedBy { get; set; }

    protected SoftDeletableEntity() : base()
    {
        IsDeleted = false;
    }

    /// <summary>Marca a entidade como excluída logicamente.</summary>
    public void MarkAsDeleted(string? user, DateTime nowUtc)
    {
        if (IsDeleted) return;
        IsDeleted = true;
        DeletedBy = user;
        DeletedAt = nowUtc;
    }

    /// <summary>Restaura a entidade removendo a marca de exclusão lógica.</summary>
    public void Restore()
    {
        IsDeleted = false;
        DeletedBy = null;
        DeletedAt = null;
    }
}