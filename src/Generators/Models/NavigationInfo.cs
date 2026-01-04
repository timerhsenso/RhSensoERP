// =============================================================================
// RHSENSOERP GENERATOR v4.6 - NAVIGATION INFO MODEL
// =============================================================================
// Versão: 4.6 - ADICIONADO: Suporte a NavigationDisplay
// =============================================================================

namespace RhSensoERP.Generators.Models;

/// <summary>
/// Informações sobre uma propriedade de navegação (relacionamento).
/// v4.6: Inclui metadados do atributo [NavigationDisplay].
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

    // =========================================================================
    // ✅ v4.6 NOVO: NAVIGATION DISPLAY
    // =========================================================================

    /// <summary>
    /// Se tem atributo [NavigationDisplay].
    /// </summary>
    public bool HasNavigationDisplay { get; set; }

    /// <summary>
    /// Propriedade a exibir (ex: "RazaoSocial", "Nome", "Descricao").
    /// Do atributo: NavigationDisplay(Property = "RazaoSocial")
    /// </summary>
    public string DisplayProperty { get; set; } = string.Empty;

    /// <summary>
    /// Módulo da API (ex: "AdministracaoPessoal").
    /// Do atributo: NavigationDisplay(Module = "AdministracaoPessoal")
    /// Se null, usa módulo da entidade atual.
    /// </summary>
    public string? DisplayModule { get; set; }

    /// <summary>
    /// Route da entidade na API (ex: "tiposanguineo").
    /// Do atributo: NavigationDisplay(EntityRoute = "tiposanguineo")
    /// Se null, gera automaticamente.
    /// </summary>
    public string? DisplayEntityRoute { get; set; }

    /// <summary>
    /// Se deve aparecer como coluna no grid.
    /// Do atributo: NavigationDisplay(GridColumn = true)
    /// </summary>
    public bool GridColumn { get; set; }

    /// <summary>
    /// Header da coluna no grid.
    /// Do atributo: NavigationDisplay(GridHeader = "Tipo Sanguíneo")
    /// Se null, usa DisplayName da navegação.
    /// </summary>
    public string? GridHeader { get; set; }

    /// <summary>
    /// Nome da propriedade no DTO.
    /// Do atributo: NavigationDisplay(DtoPropertyName = "FornecedorNome")
    /// Se null, gera automaticamente: {NavigationName}{DisplayProperty}
    /// </summary>
    public string? DtoPropertyName { get; set; }

    /// <summary>
    /// Order da coluna no grid.
    /// Do atributo: NavigationDisplay(GridOrder = 5)
    /// </summary>
    public int GridOrder { get; set; }

    // =========================================================================
    // PROPRIEDADES CALCULADAS
    // =========================================================================

    /// <summary>
    /// Nome da propriedade no DTO (calculado automaticamente se não especificado).
    /// Formato: {NavigationName}{DisplayProperty}
    /// Ex: Fornecedor + RazaoSocial → FornecedorRazaoSocial
    /// </summary>
    public string DtoPropertyNameComputed =>
        !string.IsNullOrEmpty(DtoPropertyName)
            ? DtoPropertyName
            : $"{Name}{DisplayProperty}";

    /// <summary>
    /// Header da coluna no grid (calculado automaticamente se não especificado).
    /// </summary>
    public string GridHeaderComputed =>
        !string.IsNullOrEmpty(GridHeader)
            ? GridHeader
            : SplitPascalCase(Name);

    /// <summary>
    /// Route da API (calculado automaticamente se não especificado).
    /// </summary>
    public string EntityRouteComputed =>
        !string.IsNullOrEmpty(DisplayEntityRoute)
            ? DisplayEntityRoute
            : TargetEntity.ToLowerInvariant();

    private static string SplitPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1").Trim();

        if (result.Length > 0)
            result = char.ToUpper(result[0]) + result.Substring(1);

        return result;
    }
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