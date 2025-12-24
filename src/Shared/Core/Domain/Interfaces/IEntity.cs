// =============================================================================
// RHSENSOERP - SHARED CORE
// =============================================================================
// Arquivo: src/Shared/RhSensoERP.Shared.Core/Domain/Interfaces/IEntity.cs
// Descrição: Interfaces base para entidades do domínio
// =============================================================================

namespace RhSensoERP.Shared.Core.Domain.Interfaces;

/// <summary>
/// Interface marcadora para entidades do domínio.
/// </summary>
public interface IEntity
{
}

/// <summary>
/// Interface para entidades com chave primária tipada.
/// </summary>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
public interface IEntity<TKey> : IEntity
    where TKey : notnull
{
    /// <summary>
    /// Chave primária da entidade.
    /// </summary>
    TKey Id { get; }
}

/// <summary>
/// Interface para entidades com chave composta.
/// </summary>
public interface ICompositeKeyEntity : IEntity
{
    /// <summary>
    /// Obtém os valores que compõem a chave.
    /// </summary>
    object[] GetKeys();
}

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
    /// Data da última atualização.
    /// </summary>
    DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Usuário que atualizou.
    /// </summary>
    string? UpdatedBy { get; set; }
}

/// <summary>
/// Interface para entidades com soft delete.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Indica se o registro foi excluído logicamente.
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// Data da exclusão lógica.
    /// </summary>
    DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Usuário que excluiu.
    /// </summary>
    string? DeletedBy { get; set; }
}

/// <summary>
/// Interface para entidades multi-tenant.
/// </summary>
public interface ITenantEntity
{
    /// <summary>
    /// ID do tenant (empresa/filial).
    /// </summary>
    int TenantId { get; set; }
}

/// <summary>
/// Interface para entidades com controle de versão (concorrência otimista).
/// </summary>
public interface IVersionedEntity
{
    /// <summary>
    /// Versão do registro para controle de concorrência.
    /// </summary>
    byte[] RowVersion { get; set; }
}

/// <summary>
/// Interface combinada: auditável + soft delete + tenant.
/// </summary>
public interface IFullAuditedEntity : IAuditableEntity, ISoftDeletable, ITenantEntity
{
}

/// <summary>
/// Interface combinada com chave tipada.
/// </summary>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
public interface IFullAuditedEntity<TKey> : IEntity<TKey>, IFullAuditedEntity
    where TKey : notnull
{
}
