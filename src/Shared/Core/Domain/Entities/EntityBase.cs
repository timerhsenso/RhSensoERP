// =============================================================================
// RHSENSOERP - SHARED CORE
// =============================================================================
// Arquivo: src/Shared/RhSensoERP.Shared.Core/Domain/Entities/EntityBase.cs
// Descrição: Classes base abstratas para entidades do domínio
// =============================================================================

using RhSensoERP.Shared.Core.Domain.Interfaces;

namespace RhSensoERP.Shared.Core.Domain.Entities;

/// <summary>
/// Classe base abstrata para entidades com chave primária.
/// </summary>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
public abstract class EntityBase<TKey> : IEntity<TKey>
    where TKey : notnull
{
    /// <inheritdoc />
    public virtual TKey Id { get; protected set; } = default!;

    /// <summary>
    /// Verifica igualdade por ID.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not EntityBase<TKey> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (IsTransient() || other.IsTransient())
            return false;

        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Verifica se a entidade é transiente (sem ID definido).
    /// </summary>
    public bool IsTransient()
    {
        return Id.Equals(default(TKey));
    }

    /// <summary>
    /// Hash code baseado no ID.
    /// </summary>
    public override int GetHashCode()
    {
        return IsTransient()
            ? base.GetHashCode()
            : Id.GetHashCode() ^ 31;
    }

    public static bool operator ==(EntityBase<TKey>? left, EntityBase<TKey>? right)
    {
        return left?.Equals(right) ?? right is null;
    }

    public static bool operator !=(EntityBase<TKey>? left, EntityBase<TKey>? right)
    {
        return !(left == right);
    }
}

/// <summary>
/// Classe base com auditoria.
/// </summary>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
public abstract class AuditableEntityBase<TKey> : EntityBase<TKey>, IAuditableEntity
    where TKey : notnull
{
    /// <inheritdoc />
    public DateTime CreatedAt { get; set; }

    /// <inheritdoc />
    public string? CreatedBy { get; set; }

    /// <inheritdoc />
    public DateTime? UpdatedAt { get; set; }

    /// <inheritdoc />
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// Classe base com soft delete.
/// </summary>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
public abstract class SoftDeletableEntityBase<TKey> : AuditableEntityBase<TKey>, ISoftDeletable
    where TKey : notnull
{
    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <inheritdoc />
    public DateTime? DeletedAt { get; set; }

    /// <inheritdoc />
    public string? DeletedBy { get; set; }

    /// <summary>
    /// Marca o registro como excluído.
    /// </summary>
    public virtual void SoftDelete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Restaura o registro excluído.
    /// </summary>
    public virtual void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}

/// <summary>
/// Classe base completa: auditável + soft delete + tenant.
/// </summary>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
public abstract class FullAuditedEntityBase<TKey> : SoftDeletableEntityBase<TKey>, ITenantEntity
    where TKey : notnull
{
    /// <inheritdoc />
    public int TenantId { get; set; }
}

/// <summary>
/// Classe base para entidades com chave string (códigos alfanuméricos).
/// </summary>
public abstract class CodeEntityBase : EntityBase<string>
{
    /// <summary>
    /// Alias para Id (para clareza semântica com códigos).
    /// </summary>
    public string Codigo
    {
        get => Id;
        set => Id = value;
    }
}

/// <summary>
/// Classe base para entidades com chave int (auto-increment).
/// </summary>
public abstract class IntEntityBase : AuditableEntityBase<int>
{
}

/// <summary>
/// Classe base para entidades com chave Guid.
/// </summary>
public abstract class GuidEntityBase : AuditableEntityBase<Guid>
{
    protected GuidEntityBase()
    {
        Id = Guid.NewGuid();
    }
}
