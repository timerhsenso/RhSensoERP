using System;

namespace RhSensoERP.Shared.Application.Interfaces;

/// <summary>
/// Marca um command que requer validação de unicidade.
/// O Behavior do MediatR irá validar automaticamente.
/// </summary>
public interface IUniqueValidatable
{
    /// <summary>
    /// Tipo da entidade que será validada.
    /// </summary>
    Type EntityType { get; }

    /// <summary>
    /// ID da entidade (null para criação, preenchido para edição).
    /// </summary>
    object? EntityId { get; }

    /// <summary>
    /// TenantId do contexto (para validação multi-tenant).
    /// </summary>
    Guid? TenantId { get; }
}