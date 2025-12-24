// =============================================================================
// RHSENSOERP - SHARED APPLICATION
// =============================================================================
// Arquivo: src/Shared/RhSensoERP.Shared.Application/Specifications/ISpecification.cs
// Descrição: Padrão Specification para queries complexas
// =============================================================================

using System.Linq.Expressions;

namespace RhSensoERP.Shared.Application.Specifications;

/// <summary>
/// Interface do padrão Specification para encapsular critérios de query.
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public interface ISpecification<T> where T : class
{
    /// <summary>
    /// Expressão de filtro (WHERE).
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// Lista de includes (navegação).
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Lista de includes por string (para ThenInclude).
    /// </summary>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// Expressão de ordenação ascendente.
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// Expressão de ordenação descendente.
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// Ordenações adicionais (ThenBy).
    /// </summary>
    List<(Expression<Func<T, object>> KeySelector, bool Descending)> ThenByExpressions { get; }

    /// <summary>
    /// Número de registros a pular (paginação).
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// Número de registros a retornar (paginação).
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// Indica se paginação está habilitada.
    /// </summary>
    bool IsPagingEnabled { get; }

    /// <summary>
    /// Indica se deve usar tracking (EF Core).
    /// </summary>
    bool AsNoTracking { get; }

    /// <summary>
    /// Indica se deve usar split query para collections.
    /// </summary>
    bool AsSplitQuery { get; }

    /// <summary>
    /// Indica se deve ignorar query filters globais.
    /// </summary>
    bool IgnoreQueryFilters { get; }
}

/// <summary>
/// Implementação base do padrão Specification.
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public abstract class Specification<T> : ISpecification<T> where T : class
{
    /// <inheritdoc />
    public Expression<Func<T, bool>>? Criteria { get; private set; }

    /// <inheritdoc />
    public List<Expression<Func<T, object>>> Includes { get; } = new();

    /// <inheritdoc />
    public List<string> IncludeStrings { get; } = new();

    /// <inheritdoc />
    public Expression<Func<T, object>>? OrderBy { get; private set; }

    /// <inheritdoc />
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    /// <inheritdoc />
    public List<(Expression<Func<T, object>> KeySelector, bool Descending)> ThenByExpressions { get; } = new();

    /// <inheritdoc />
    public int? Skip { get; private set; }

    /// <inheritdoc />
    public int? Take { get; private set; }

    /// <inheritdoc />
    public bool IsPagingEnabled { get; private set; }

    /// <inheritdoc />
    public bool AsNoTracking { get; private set; } = true;

    /// <inheritdoc />
    public bool AsSplitQuery { get; private set; }

    /// <inheritdoc />
    public bool IgnoreQueryFilters { get; private set; }

    /// <summary>
    /// Define o critério de filtro.
    /// </summary>
    protected void AddCriteria(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// Adiciona um include.
    /// </summary>
    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    /// <summary>
    /// Adiciona um include por string.
    /// </summary>
    protected void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    /// <summary>
    /// Define ordenação ascendente.
    /// </summary>
    protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    /// <summary>
    /// Define ordenação descendente.
    /// </summary>
    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
        OrderByDescending = orderByDescExpression;
    }

    /// <summary>
    /// Adiciona ordenação secundária.
    /// </summary>
    protected void ApplyThenBy(Expression<Func<T, object>> thenByExpression, bool descending = false)
    {
        ThenByExpressions.Add((thenByExpression, descending));
    }

    /// <summary>
    /// Define paginação.
    /// </summary>
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    /// <summary>
    /// Define se deve usar tracking.
    /// </summary>
    protected void ApplyTracking(bool tracking)
    {
        AsNoTracking = !tracking;
    }

    /// <summary>
    /// Define se deve usar split query.
    /// </summary>
    protected void ApplySplitQuery(bool splitQuery = true)
    {
        AsSplitQuery = splitQuery;
    }

    /// <summary>
    /// Define se deve ignorar query filters.
    /// </summary>
    protected void ApplyIgnoreQueryFilters(bool ignore = true)
    {
        IgnoreQueryFilters = ignore;
    }
}

/// <summary>
/// Specification genérica construível via fluent API.
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public class GenericSpecification<T> : Specification<T> where T : class
{
    /// <summary>
    /// Adiciona filtro WHERE.
    /// </summary>
    public GenericSpecification<T> Where(Expression<Func<T, bool>> criteria)
    {
        AddCriteria(criteria);
        return this;
    }

    /// <summary>
    /// Adiciona Include.
    /// </summary>
    public GenericSpecification<T> Include(Expression<Func<T, object>> includeExpression)
    {
        AddInclude(includeExpression);
        return this;
    }

    /// <summary>
    /// Adiciona Include por string.
    /// </summary>
    public GenericSpecification<T> Include(string navigationPropertyPath)
    {
        AddInclude(navigationPropertyPath);
        return this;
    }

    /// <summary>
    /// Define ordenação ascendente.
    /// </summary>
    public new GenericSpecification<T> OrderBy(Expression<Func<T, object>> orderByExpression)
    {
        ApplyOrderBy(orderByExpression);
        return this;
    }

    /// <summary>
    /// Define ordenação descendente.
    /// </summary>
    public new GenericSpecification<T> OrderByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
        ApplyOrderByDescending(orderByDescExpression);
        return this;
    }

    /// <summary>
    /// Adiciona ordenação secundária ascendente.
    /// </summary>
    public GenericSpecification<T> ThenBy(Expression<Func<T, object>> thenByExpression)
    {
        ApplyThenBy(thenByExpression, false);
        return this;
    }

    /// <summary>
    /// Adiciona ordenação secundária descendente.
    /// </summary>
    public GenericSpecification<T> ThenByDescending(Expression<Func<T, object>> thenByExpression)
    {
        ApplyThenBy(thenByExpression, true);
        return this;
    }

    /// <summary>
    /// Define paginação por página/tamanho.
    /// </summary>
    public GenericSpecification<T> Page(int pageNumber, int pageSize)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        return this;
    }

    /// <summary>
    /// Define paginação por skip/take.
    /// </summary>
    public GenericSpecification<T> Paginate(int skip, int take)
    {
        ApplyPaging(skip, take);
        return this;
    }

    /// <summary>
    /// Habilita/desabilita tracking.
    /// </summary>
    public GenericSpecification<T> WithTracking(bool tracking = true)
    {
        ApplyTracking(tracking);
        return this;
    }

    /// <summary>
    /// Habilita split query.
    /// </summary>
    public GenericSpecification<T> AsSplit()
    {
        ApplySplitQuery(true);
        return this;
    }

    /// <summary>
    /// Ignora query filters globais.
    /// </summary>
    public GenericSpecification<T> IgnoreFilters()
    {
        ApplyIgnoreQueryFilters(true);
        return this;
    }
}
