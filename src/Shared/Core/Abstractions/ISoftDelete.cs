namespace RhSensoERP.Shared.Core.Abstractions;

/// <summary>Marca entidades que suportam soft delete.</summary>
public interface ISoftDelete
{
    /// <summary>Indica se o registro está logicamente excluído.</summary>
    bool IsDeleted { get; set; }

    /// <summary>Data e hora da exclusão lógica.</summary>
    DateTime? DeletedAt { get; set; }

    /// <summary>Usuário que excluiu o registro.</summary>
    string? DeletedBy { get; set; }
}