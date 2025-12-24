// =============================================================================
// RHSENSOERP GENERATOR v3.3 - NAVIGATION INFO MODEL
// =============================================================================
// Arquivo: src/Generators/Models/NavigationInfo.cs
// Versão: 3.3 - Suporte a navegações/relacionamentos
// =============================================================================

namespace RhSensoERP.Generators.Models;

/// <summary>
/// Informações sobre uma propriedade de navegação (relacionamento).
/// </summary>
public class NavigationInfo
{
    /// <summary>
    /// Nome da propriedade de navegação (ex: "Banco").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tipo da entidade relacionada (ex: "Banco").
    /// </summary>
    public string TargetEntity { get; set; } = string.Empty;

    /// <summary>
    /// Namespace completo da entidade relacionada.
    /// </summary>
    public string TargetEntityFullName { get; set; } = string.Empty;

    /// <summary>
    /// Nome da propriedade FK (ex: "Idbanco", "IdBanco").
    /// </summary>
    public string ForeignKeyProperty { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de relacionamento.
    /// </summary>
    public NavigationRelationshipType RelationshipType { get; set; } = NavigationRelationshipType.ManyToOne;

    /// <summary>
    /// Se é nullable (opcional).
    /// </summary>
    public bool IsNullable { get; set; } = true;

    /// <summary>
    /// Nome da propriedade de coleção inversa (para HasMany).
    /// Ex: "AgenciaList" em Banco.
    /// </summary>
    public string? InverseProperty { get; set; }

    /// <summary>
    /// Comportamento de delete.
    /// </summary>
    public NavigationDeleteBehavior OnDelete { get; set; } = NavigationDeleteBehavior.Restrict;
}

/// <summary>
/// Tipo de relacionamento.
/// </summary>
public enum NavigationRelationshipType
{
    /// <summary>
    /// Muitos para Um (ex: Agencia -> Banco).
    /// Propriedade: public virtual Banco Banco { get; set; }
    /// </summary>
    ManyToOne,

    /// <summary>
    /// Um para Muitos (ex: Banco -> Agencias).
    /// Propriedade: public virtual ICollection&lt;Agencia&gt; Agencias { get; set; }
    /// </summary>
    OneToMany,

    /// <summary>
    /// Um para Um.
    /// </summary>
    OneToOne
}

/// <summary>
/// Comportamento de delete para FK.
/// </summary>
public enum NavigationDeleteBehavior
{
    Restrict,
    Cascade,
    SetNull,
    NoAction
}
