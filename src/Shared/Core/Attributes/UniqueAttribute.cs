using System;

namespace RhSensoERP.Shared.Core.Attributes;

/// <summary>
/// Marca uma propriedade como única.
/// O Source Generator criará automaticamente:
/// - Índice único no EntityConfiguration
/// - Validação de duplicatas no pipeline MediatR
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class UniqueAttribute : Attribute
{
    /// <summary>
    /// Escopo da unicidade.
    /// </summary>
    public UniqueScope Scope { get; set; } = UniqueScope.Tenant;

    /// <summary>
    /// Nome customizado para mensagens de erro.
    /// Se não especificado, usa DisplayName da propriedade.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Mensagem de erro customizada.
    /// Placeholders: {PropertyName}, {Value}
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Permite valores nulos na validação de unicidade.
    /// </summary>
    public bool AllowNull { get; set; } = true;

    public UniqueAttribute()
    {
    }

    public UniqueAttribute(UniqueScope scope)
    {
        Scope = scope;
    }

    public UniqueAttribute(UniqueScope scope, string displayName)
    {
        Scope = scope;
        DisplayName = displayName;
    }
}

/// <summary>
/// Escopo da validação de unicidade.
/// </summary>
public enum UniqueScope
{
    /// <summary>
    /// Validação global (toda a tabela).
    /// </summary>
    Global = 0,

    /// <summary>
    /// Validação por tenant (considera TenantId).
    /// </summary>
    Tenant = 1
}