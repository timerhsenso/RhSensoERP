// =============================================================================
// RHSENSOERP - SHARED APPLICATION
// =============================================================================
// Arquivo: src/Shared/RhSensoERP.Shared.Application/Services/GenericService.cs
// Descrição: Implementação genérica do serviço CRUD
// =============================================================================

using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using RhSensoERP.Shared.Core.Common;
using RhSensoERP.Shared.Core.Abstractions;
using RhSensoERP.Shared.Application.Interfaces;

namespace RhSensoERP.Shared.Application.Services;

/// <summary>
/// Implementação genérica do serviço CRUD.
/// Fornece operações padrão que podem ser sobrescritas em classes derivadas.
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
/// <typeparam name="TDto">Tipo do DTO de leitura</typeparam>
/// <typeparam name="TCreateDto">Tipo do DTO de criação</typeparam>
/// <typeparam name="TUpdateDto">Tipo do DTO de atualização</typeparam>
public abstract class GenericService<TEntity, TKey, TDto, TCreateDto, TUpdateDto>
    : ICrudService<TEntity, TKey, TDto, TCreateDto, TUpdateDto>
    where TEntity : class
    where TKey : notnull
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    protected readonly IRepositoryWithKey<TEntity, TKey> Repository;
    protected readonly IUnitOfWork UnitOfWork;
    protected readonly ILogger Logger;

    /// <summary>
    /// Construtor.
    /// </summary>
    protected GenericService(
        IRepositoryWithKey<TEntity, TKey> repository,
        IUnitOfWork unitOfWork,
        ILogger logger)
    {
        Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Abstract Methods (devem ser implementados)

    /// <summary>
    /// Mapeia uma entidade para DTO.
    /// </summary>
    protected abstract TDto MapToDto(TEntity entity);

    /// <summary>
    /// Mapeia um DTO de criação para entidade.
    /// </summary>
    protected abstract TEntity MapToEntity(TCreateDto createDto);

    /// <summary>
    /// Atualiza uma entidade com dados do DTO de atualização.
    /// </summary>
    protected abstract void UpdateEntity(TEntity entity, TUpdateDto updateDto);

    /// <summary>
    /// Obtém a chave primária de uma entidade.
    /// </summary>
    protected abstract TKey GetEntityKey(TEntity entity);

    #endregion

    #region Virtual Methods (podem ser sobrescritos)

    /// <summary>
    /// Valida o DTO de criação antes de criar a entidade.
    /// </summary>
    protected virtual Task<Result> ValidateCreateAsync(TCreateDto createDto, CancellationToken ct)
    {
        return Task.FromResult(Result.Success());
    }

    /// <summary>
    /// Valida o DTO de atualização antes de atualizar a entidade.
    /// </summary>
    protected virtual Task<Result> ValidateUpdateAsync(TKey id, TUpdateDto updateDto, TEntity existingEntity, CancellationToken ct)
    {
        return Task.FromResult(Result.Success());
    }

    /// <summary>
    /// Valida antes de excluir a entidade.
    /// </summary>
    protected virtual Task<Result> ValidateDeleteAsync(TKey id, TEntity existingEntity, CancellationToken ct)
    {
        return Task.FromResult(Result.Success());
    }

    /// <summary>
    /// Executado após criar a entidade (hook).
    /// </summary>
    protected virtual Task OnAfterCreateAsync(TEntity entity, TCreateDto createDto, CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Executado após atualizar a entidade (hook).
    /// </summary>
    protected virtual Task OnAfterUpdateAsync(TEntity entity, TUpdateDto updateDto, CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Executado após excluir a entidade (hook).
    /// </summary>
    protected virtual Task OnAfterDeleteAsync(TKey id, CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Aplica filtros customizados na consulta paginada.
    /// </summary>
    protected virtual Expression<Func<TEntity, bool>>? GetPagedFilter(ServicePagedRequest request)
    {
        return null;
    }

    /// <summary>
    /// Aplica busca global (SearchTerm) na consulta.
    /// </summary>
    protected virtual Expression<Func<TEntity, bool>>? GetSearchFilter(string searchTerm)
    {
        return null;
    }

    #endregion

    #region IReadService

    /// <inheritdoc />
    public virtual async Task<TDto?> GetByIdAsync(TKey id, CancellationToken ct = default)
    {
        Logger.LogDebug("Buscando {Entity} por ID: {Id}", typeof(TEntity).Name, id);

        var entity = await Repository.GetByIdAsync(id, ct);
        if (entity == null)
        {
            Logger.LogDebug("{Entity} não encontrado: {Id}", typeof(TEntity).Name, id);
            return null;
        }

        return MapToDto(entity);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TDto>> GetAllAsync(CancellationToken ct = default)
    {
        Logger.LogDebug("Listando todos os {Entity}", typeof(TEntity).Name);

        var entities = await Repository.GetAllAsync(ct);
        return entities.Select(MapToDto).ToList();
    }

    /// <inheritdoc />
    public virtual async Task<ServicePagedResult<TDto>> GetPagedAsync(
        ServicePagedRequest request,
        CancellationToken ct = default)
    {
        Logger.LogDebug("Listando {Entity} paginado - Página: {Page}, Tamanho: {Size}",
            typeof(TEntity).Name, request.PageNumber, request.PageSize);

        // Construir filtro base
        Expression<Func<TEntity, bool>>? filter = GetPagedFilter(request);

        // Aplicar busca global
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchFilter = GetSearchFilter(request.SearchTerm);
            if (searchFilter != null)
            {
                filter = filter == null
                    ? searchFilter
                    : CombineFilters(filter, searchFilter);
            }
        }

        // Contar total
        var totalCount = filter != null
            ? await Repository.CountAsync(filter, ct)
            : await Repository.CountAsync(ct: ct);

        // Buscar itens
        var query = Repository.Query();

        if (filter != null)
            query = query.Where(filter);

        // Aplicar paginação
        var items = await Task.Run(() =>
            query.Skip(request.Skip).Take(request.PageSize).ToList(), ct);

        var dtos = items.Select(MapToDto).ToList();

        return new ServicePagedResult<TDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }

    /// <inheritdoc />
    public virtual async Task<bool> ExistsAsync(TKey id, CancellationToken ct = default)
    {
        var entity = await Repository.GetByIdAsync(id, ct);
        return entity != null;
    }

    /// <inheritdoc />
    public virtual async Task<int> CountAsync(CancellationToken ct = default)
    {
        return await Repository.CountAsync(ct: ct);
    }

    #endregion

    #region IWriteService

    /// <inheritdoc />
    public virtual async Task<Result<TDto>> CreateAsync(TCreateDto createDto, CancellationToken ct = default)
    {
        Logger.LogDebug("Criando {Entity}", typeof(TEntity).Name);

        // Validar
        var validationResult = await ValidateCreateAsync(createDto, ct);
        if (!validationResult.IsSuccess)
        {
            Logger.LogWarning("Validação falhou ao criar {Entity}: {Error}",
                typeof(TEntity).Name, validationResult.Error);
            return Result<TDto>.Failure(validationResult.Error);
        }

        try
        {
            // Mapear e adicionar
            var entity = MapToEntity(createDto);
            await Repository.AddAsync(entity, ct);
            await UnitOfWork.SaveChangesAsync(ct);

            // Hook pós-criação
            await OnAfterCreateAsync(entity, createDto, ct);

            var dto = MapToDto(entity);
            Logger.LogInformation("{Entity} criado com sucesso: {Id}",
                typeof(TEntity).Name, GetEntityKey(entity));

            return Result<TDto>.Success(dto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao criar {Entity}", typeof(TEntity).Name);
            return Result<TDto>.Failure("General.Error", $"Erro ao criar registro: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<TDto>> UpdateAsync(TKey id, TUpdateDto updateDto, CancellationToken ct = default)
    {
        Logger.LogDebug("Atualizando {Entity}: {Id}", typeof(TEntity).Name, id);

        // Buscar entidade existente
        var entity = await Repository.GetByIdAsync(id, ct);
        if (entity == null)
        {
            Logger.LogWarning("{Entity} não encontrado para atualização: {Id}",
                typeof(TEntity).Name, id);
            // ✅ CORRIGIDO: Linha 322 - Usa Error.NotFound() ao invés de new Error()
            return Result<TDto>.Failure(Error.NotFound("NotFound", $"Registro não encontrado: {id}"));
        }

        // Validar
        var validationResult = await ValidateUpdateAsync(id, updateDto, entity, ct);
        if (!validationResult.IsSuccess)
        {
            Logger.LogWarning("Validação falhou ao atualizar {Entity}: {Error}",
                typeof(TEntity).Name, validationResult.Error);
            return Result<TDto>.Failure(validationResult.Error);
        }

        try
        {
            // Atualizar entidade
            UpdateEntity(entity, updateDto);
            await Repository.UpdateAsync(entity, ct);
            await UnitOfWork.SaveChangesAsync(ct);

            // Hook pós-atualização
            await OnAfterUpdateAsync(entity, updateDto, ct);

            var dto = MapToDto(entity);
            Logger.LogInformation("{Entity} atualizado com sucesso: {Id}",
                typeof(TEntity).Name, id);

            return Result<TDto>.Success(dto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao atualizar {Entity}: {Id}", typeof(TEntity).Name, id);
            return Result<TDto>.Failure("General.Error", $"Erro ao atualizar registro: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result> DeleteAsync(TKey id, CancellationToken ct = default)
    {
        Logger.LogDebug("Excluindo {Entity}: {Id}", typeof(TEntity).Name, id);

        // Buscar entidade existente
        var entity = await Repository.GetByIdAsync(id, ct);
        if (entity == null)
        {
            Logger.LogWarning("{Entity} não encontrado para exclusão: {Id}",
                typeof(TEntity).Name, id);
            // ✅ CORRIGIDO: Linha 350 - Usa Error.NotFound() ao invés de new Error()
            return Result.Failure(Error.NotFound("NotFound", $"Registro não encontrado: {id}"));
        }

        // Validar
        var validationResult = await ValidateDeleteAsync(id, entity, ct);
        if (!validationResult.IsSuccess)
        {
            Logger.LogWarning("Validação falhou ao excluir {Entity}: {Error}",
                typeof(TEntity).Name, validationResult.Error);
            return validationResult;
        }

        try
        {
            await Repository.DeleteAsync(entity, ct);
            await UnitOfWork.SaveChangesAsync(ct);

            // Hook pós-exclusão
            await OnAfterDeleteAsync(id, ct);

            Logger.LogInformation("{Entity} excluído com sucesso: {Id}",
                typeof(TEntity).Name, id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao excluir {Entity}: {Id}", typeof(TEntity).Name, id);
            // ✅ CORRIGIDO: Usa Error.Failure() ao invés de new Error()
            return Result.Failure(Error.Failure("General.Error", $"Erro ao excluir registro: {ex.Message}"));
        }
    }

    /// <inheritdoc />
    public virtual async Task<Result<int>> DeleteRangeAsync(IEnumerable<TKey> ids, CancellationToken ct = default)
    {
        var idList = ids.ToList();
        Logger.LogDebug("Excluindo {Count} {Entity}", idList.Count, typeof(TEntity).Name);

        var deletedCount = 0;
        var errors = new List<string>();

        foreach (var id in idList)
        {
            var result = await DeleteAsync(id, ct);
            if (result.IsSuccess)
            {
                deletedCount++;
            }
            else
            {
                errors.Add($"ID {id}: {result.Error.Message}");
            }
        }

        if (errors.Any())
        {
            Logger.LogWarning("Exclusão em lote parcial: {Deleted}/{Total}. Erros: {Errors}",
                deletedCount, idList.Count, string.Join("; ", errors));

            if (deletedCount == 0)
            {
                return Result<int>.Failure("DeleteRange.Failed", string.Join("; ", errors));
            }
        }

        return Result<int>.Success(deletedCount);
    }

    #endregion

    #region Protected Helpers

    /// <summary>
    /// Combina duas expressões de filtro com AND.
    /// </summary>
    protected Expression<Func<TEntity, bool>> CombineFilters(
        Expression<Func<TEntity, bool>> filter1,
        Expression<Func<TEntity, bool>> filter2)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "e");

        var body1 = ReplaceParameter(filter1.Body, filter1.Parameters[0], parameter);
        var body2 = ReplaceParameter(filter2.Body, filter2.Parameters[0], parameter);

        var combined = Expression.AndAlso(body1, body2);

        return Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
    }

    private Expression ReplaceParameter(Expression expression, ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        return new ParameterReplacer(oldParameter, newParameter).Visit(expression);
    }

    private class ParameterReplacer : System.Linq.Expressions.ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }
    }

    #endregion
}

/// <summary>
/// Serviço genérico simplificado (mesmo DTO para todas as operações).
/// </summary>
public abstract class GenericService<TEntity, TKey, TDto>
    : GenericService<TEntity, TKey, TDto, TDto, TDto>, ICrudService<TEntity, TKey, TDto>
    where TEntity : class
    where TKey : notnull
    where TDto : class
{
    protected GenericService(
        IRepositoryWithKey<TEntity, TKey> repository,
        IUnitOfWork unitOfWork,
        ILogger logger)
        : base(repository, unitOfWork, logger)
    {
    }
}