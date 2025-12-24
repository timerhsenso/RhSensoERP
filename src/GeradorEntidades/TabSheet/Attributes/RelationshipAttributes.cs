// =============================================================================
// TABSHEET GENERATOR - ATRIBUTOS DE RELACIONAMENTO
// Versão: 1.0.0
// Autor: RhSensoERP Team
// Data: 2024
// 
// IMPORTANTE: Este arquivo é ISOLADO do gerador CRUD existente.
// Estes atributos permitem ao Source Generator detectar relacionamentos
// e gerar o código de backend apropriado (Fluent API, Endpoints, etc.).
// =============================================================================

namespace GeradorEntidades.TabSheet.Attributes;

#region Atributos de Entidade

/// <summary>
/// Marca uma entidade como MESTRE em um relacionamento mestre/detalhe.
/// O Source Generator usará este atributo para gerar:
/// - Configuração Fluent API com HasMany()
/// - Endpoints para buscar registros relacionados
/// - Includes automáticos nos repositórios
/// </summary>
/// <remarks>
/// <para>
/// Exemplo de uso:
/// <code>
/// [MasterEntity]
/// [Table("RH_Funcionario")]
/// public class Funcionario
/// {
///     public Guid IdFuncionario { get; set; }
///     public string Nome { get; set; }
///     
///     // Coleções de detalhes
///     [DetailCollection(typeof(FuncionarioDependente), "IdFuncionario")]
///     public virtual ICollection&lt;FuncionarioDependente&gt; Dependentes { get; set; }
/// }
/// </code>
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class MasterEntityAttribute : Attribute
{
    /// <summary>
    /// Nome amigável da entidade mestre (para UI e documentação).
    /// </summary>
    /// <example>Funcionário</example>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Coluna usada para exibição em combos e referências.
    /// Se não informado, tenta usar: Nome, Descricao, Titulo, Name, Description.
    /// </summary>
    /// <example>Nome</example>
    public string? DisplayColumn { get; set; }

    /// <summary>
    /// Ícone FontAwesome para a entidade.
    /// </summary>
    /// <example>fas fa-user</example>
    public string? Icon { get; set; }

    /// <summary>
    /// Construtor padrão.
    /// </summary>
    public MasterEntityAttribute() { }

    /// <summary>
    /// Construtor com nome de exibição.
    /// </summary>
    /// <param name="displayName">Nome amigável da entidade.</param>
    public MasterEntityAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}

/// <summary>
/// Marca uma entidade como DETALHE (filha) em um relacionamento mestre/detalhe.
/// O Source Generator usará este atributo para gerar:
/// - Configuração Fluent API com HasOne().WithMany()
/// - Propriedade de navegação para o mestre
/// - Filtros automáticos por FK nos endpoints
/// </summary>
/// <remarks>
/// <para>
/// Exemplo de uso:
/// <code>
/// [DetailEntity(typeof(Funcionario), "IdFuncionario")]
/// [Table("RH_FuncionarioDependente")]
/// public class FuncionarioDependente
/// {
///     public Guid IdDependente { get; set; }
///     public Guid IdFuncionario { get; set; }  // FK
///     public string Nome { get; set; }
///     
///     // Navegação para o mestre
///     [MasterReference]
///     public virtual Funcionario Funcionario { get; set; }
/// }
/// </code>
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class DetailEntityAttribute : Attribute
{
    /// <summary>
    /// Tipo da entidade mestre.
    /// </summary>
    public Type MasterType { get; }

    /// <summary>
    /// Nome da coluna de chave estrangeira que referencia o mestre.
    /// </summary>
    /// <example>IdFuncionario</example>
    public string ForeignKey { get; }

    /// <summary>
    /// Nome amigável da entidade detalhe (para UI e documentação).
    /// </summary>
    /// <example>Dependente</example>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Nome da propriedade de navegação para o mestre.
    /// Se não informado, usa o nome do tipo mestre.
    /// </summary>
    /// <example>Funcionario</example>
    public string? NavigationPropertyName { get; set; }

    /// <summary>
    /// Se a FK é obrigatória (relacionamento obrigatório).
    /// Default: true.
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    /// Comportamento ao deletar o mestre.
    /// </summary>
    public DeleteBehavior OnDelete { get; set; } = DeleteBehavior.Cascade;

    /// <summary>
    /// Construtor obrigatório.
    /// </summary>
    /// <param name="masterType">Tipo da entidade mestre.</param>
    /// <param name="foreignKey">Nome da coluna FK.</param>
    public DetailEntityAttribute(Type masterType, string foreignKey)
    {
        MasterType = masterType ?? throw new ArgumentNullException(nameof(masterType));
        ForeignKey = foreignKey ?? throw new ArgumentNullException(nameof(foreignKey));
    }
}

#endregion

#region Atributos de Propriedade

/// <summary>
/// Marca uma propriedade de coleção como relacionamento 1:N com entidades detalhe.
/// Usado na entidade MESTRE para indicar a coleção de filhos.
/// </summary>
/// <remarks>
/// <para>
/// Exemplo de uso:
/// <code>
/// [MasterEntity]
/// public class Funcionario
/// {
///     public Guid IdFuncionario { get; set; }
///     
///     [DetailCollection(typeof(FuncionarioDependente), "IdFuncionario")]
///     public virtual ICollection&lt;FuncionarioDependente&gt; Dependentes { get; set; }
///     
///     [DetailCollection(typeof(FuncionarioDocumento), "IdFuncionario", TabTitle = "Documentos")]
///     public virtual ICollection&lt;FuncionarioDocumento&gt; Documentos { get; set; }
/// }
/// </code>
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class DetailCollectionAttribute : Attribute
{
    /// <summary>
    /// Tipo da entidade detalhe (filha).
    /// </summary>
    public Type DetailType { get; }

    /// <summary>
    /// Nome da coluna FK na entidade detalhe.
    /// </summary>
    public string ForeignKey { get; }

    /// <summary>
    /// Título da aba no TabSheet (se diferente do nome da propriedade).
    /// </summary>
    /// <example>Dependentes</example>
    public string? TabTitle { get; set; }

    /// <summary>
    /// Ícone FontAwesome para a aba.
    /// </summary>
    /// <example>fas fa-users</example>
    public string? TabIcon { get; set; }

    /// <summary>
    /// Ordem da aba no TabSheet.
    /// </summary>
    public int TabOrder { get; set; } = 0;

    /// <summary>
    /// Se a aba está habilitada por padrão.
    /// </summary>
    public bool TabEnabled { get; set; } = true;

    /// <summary>
    /// Se deve carregar os dados via AJAX (lazy load).
    /// </summary>
    public bool LazyLoad { get; set; } = true;

    /// <summary>
    /// Construtor obrigatório.
    /// </summary>
    /// <param name="detailType">Tipo da entidade detalhe.</param>
    /// <param name="foreignKey">Nome da coluna FK.</param>
    public DetailCollectionAttribute(Type detailType, string foreignKey)
    {
        DetailType = detailType ?? throw new ArgumentNullException(nameof(detailType));
        ForeignKey = foreignKey ?? throw new ArgumentNullException(nameof(foreignKey));
    }
}

/// <summary>
/// Marca uma propriedade de navegação como referência ao MESTRE.
/// Usado na entidade DETALHE para indicar a referência ao pai.
/// </summary>
/// <remarks>
/// <para>
/// Exemplo de uso:
/// <code>
/// [DetailEntity(typeof(Funcionario), "IdFuncionario")]
/// public class FuncionarioDependente
/// {
///     public Guid IdFuncionario { get; set; }
///     
///     [MasterReference]
///     public virtual Funcionario Funcionario { get; set; }
/// }
/// </code>
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class MasterReferenceAttribute : Attribute
{
    /// <summary>
    /// Nome da coluna FK (se diferente do padrão).
    /// </summary>
    public string? ForeignKeyProperty { get; set; }

    /// <summary>
    /// Se deve incluir automaticamente em queries (Include).
    /// </summary>
    public bool AutoInclude { get; set; } = false;

    /// <summary>
    /// Construtor padrão.
    /// </summary>
    public MasterReferenceAttribute() { }

    /// <summary>
    /// Construtor com FK explícita.
    /// </summary>
    /// <param name="foreignKeyProperty">Nome da propriedade FK.</param>
    public MasterReferenceAttribute(string foreignKeyProperty)
    {
        ForeignKeyProperty = foreignKeyProperty;
    }
}

#endregion

#region Atributos de Configuração de UI

/// <summary>
/// Configura uma aba específica no TabSheet.
/// Pode ser aplicado à entidade detalhe ou à propriedade de coleção no mestre.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
public class TabConfigAttribute : Attribute
{
    /// <summary>
    /// Título da aba.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Ícone FontAwesome.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Ordem de exibição.
    /// </summary>
    public int Order { get; set; } = 0;

    /// <summary>
    /// Se a aba está habilitada.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Se permite criar novos registros.
    /// </summary>
    public bool AllowCreate { get; set; } = true;

    /// <summary>
    /// Se permite editar registros.
    /// </summary>
    public bool AllowEdit { get; set; } = true;

    /// <summary>
    /// Se permite excluir registros.
    /// </summary>
    public bool AllowDelete { get; set; } = true;

    /// <summary>
    /// Se exibe badge com contagem.
    /// </summary>
    public bool ShowBadge { get; set; } = true;

    /// <summary>
    /// Classes CSS adicionais.
    /// </summary>
    public string? CssClass { get; set; }
}

/// <summary>
/// Ignora uma propriedade de coleção na geração de TabSheet.
/// Útil quando há múltiplas coleções mas nem todas devem virar abas.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class IgnoreTabAttribute : Attribute { }

#endregion

#region Enums de Suporte

/// <summary>
/// Comportamento ao deletar a entidade mestre.
/// Equivalente ao EF Core DeleteBehavior.
/// </summary>
public enum DeleteBehavior
{
    /// <summary>
    /// Deleta os registros filhos automaticamente.
    /// </summary>
    Cascade,

    /// <summary>
    /// Impede a deleção se houver filhos.
    /// </summary>
    Restrict,

    /// <summary>
    /// Define a FK como NULL nos filhos.
    /// </summary>
    SetNull,

    /// <summary>
    /// Sem ação (cuidado com integridade referencial).
    /// </summary>
    NoAction,

    /// <summary>
    /// Usa o comportamento padrão do EF/banco.
    /// </summary>
    ClientSetNull
}

#endregion

#region Helpers para Reflexão

/// <summary>
/// Extensões para facilitar a leitura dos atributos via reflexão.
/// </summary>
public static class RelationshipAttributeExtensions
{
    /// <summary>
    /// Verifica se um tipo é uma entidade mestre.
    /// </summary>
    public static bool IsMasterEntity(this Type type)
    {
        return type.GetCustomAttributes(typeof(MasterEntityAttribute), false).Length > 0;
    }

    /// <summary>
    /// Verifica se um tipo é uma entidade detalhe.
    /// </summary>
    public static bool IsDetailEntity(this Type type)
    {
        return type.GetCustomAttributes(typeof(DetailEntityAttribute), false).Length > 0;
    }

    /// <summary>
    /// Obtém o atributo MasterEntity de um tipo.
    /// </summary>
    public static MasterEntityAttribute? GetMasterEntityAttribute(this Type type)
    {
        return type.GetCustomAttributes(typeof(MasterEntityAttribute), false)
            .FirstOrDefault() as MasterEntityAttribute;
    }

    /// <summary>
    /// Obtém os atributos DetailEntity de um tipo (pode ter múltiplos).
    /// </summary>
    public static IEnumerable<DetailEntityAttribute> GetDetailEntityAttributes(this Type type)
    {
        return type.GetCustomAttributes(typeof(DetailEntityAttribute), false)
            .Cast<DetailEntityAttribute>();
    }

    /// <summary>
    /// Obtém as propriedades marcadas com DetailCollection.
    /// </summary>
    public static IEnumerable<(System.Reflection.PropertyInfo Property, DetailCollectionAttribute Attribute)>
        GetDetailCollections(this Type type)
    {
        return type.GetProperties()
            .Select(p => (Property: p, Attribute: p.GetCustomAttributes(typeof(DetailCollectionAttribute), false)
                .FirstOrDefault() as DetailCollectionAttribute))
            .Where(x => x.Attribute != null)!;
    }

    /// <summary>
    /// Obtém a propriedade marcada com MasterReference.
    /// </summary>
    public static (System.Reflection.PropertyInfo Property, MasterReferenceAttribute Attribute)?
        GetMasterReference(this Type type)
    {
        var prop = type.GetProperties()
            .Select(p => (Property: p, Attribute: p.GetCustomAttributes(typeof(MasterReferenceAttribute), false)
                .FirstOrDefault() as MasterReferenceAttribute))
            .FirstOrDefault(x => x.Attribute != null);

        return prop.Attribute != null ? prop : null;
    }
}

#endregion
