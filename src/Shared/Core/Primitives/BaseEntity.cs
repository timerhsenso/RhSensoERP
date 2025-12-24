namespace RhSensoERP.Shared.Core.Primitives;

/// <summary>
/// Entidade base com campos comuns.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets or sets o ID da entidade.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets a data de criação.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets o usuário que criou.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets a data de atualização.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets o usuário que atualizou.
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntity"/> class.
    /// </summary>
    protected BaseEntity()
    {
       CreatedAt = DateTime.UtcNow;
    }
}
