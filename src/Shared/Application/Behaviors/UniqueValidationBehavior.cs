using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RhSensoERP.Shared.Application.Exceptions;
using RhSensoERP.Shared.Application.Interfaces;
using RhSensoERP.Shared.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace RhSensoERP.Shared.Application.Behaviors;

/// <summary>
/// Behavior que valida automaticamente campos únicos antes de executar o command.
/// </summary>
public class UniqueValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<UniqueValidationBehavior<TRequest, TResponse>> _logger;
    private readonly IServiceProvider _serviceProvider;

    public UniqueValidationBehavior(
        ILogger<UniqueValidationBehavior<TRequest, TResponse>> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Só valida se o request implementar IUniqueValidatable
        if (request is not IUniqueValidatable validatable)
        {
            return await next();
        }

        var entityType = validatable.EntityType;
        var entityId = validatable.EntityId;
        var tenantId = validatable.TenantId;

        _logger.LogDebug(
            "[UNIQUE-VALIDATION] Validating {EntityType} | EntityId: {EntityId} | TenantId: {TenantId}",
            entityType.Name, entityId, tenantId);

        // Busca propriedades com [Unique] na Entity
        var uniqueProperties = entityType
            .GetProperties()
            .Where(p => p.GetCustomAttribute<UniqueAttribute>() != null)
            .ToList();

        if (!uniqueProperties.Any())
        {
            _logger.LogDebug(
                "[UNIQUE-VALIDATION] Nenhuma propriedade única encontrada em {EntityType}",
                entityType.Name);
            return await next();
        }

        // Valida cada propriedade única
        foreach (var property in uniqueProperties)
        {
            var attribute = property.GetCustomAttribute<UniqueAttribute>()!;

            // Busca o valor no request (suporta Request aninhado)
            var (propertyValue, found) = GetPropertyValue(request, property.Name);

            if (!found)
            {
                _logger.LogWarning(
                    "[UNIQUE-VALIDATION] Propriedade {PropertyName} não encontrada no request",
                    property.Name);
                continue;
            }

            // Ignora validação se valor é null e AllowNull = true
            if (propertyValue == null && attribute.AllowNull)
            {
                _logger.LogDebug(
                    "[UNIQUE-VALIDATION] Propriedade {PropertyName} é null e AllowNull=true, ignorando validação",
                    property.Name);
                continue;
            }

            await ValidateUniqueConstraint(
                entityType,
                property,
                attribute,
                propertyValue,
                entityId,
                tenantId,
                cancellationToken);
        }

        return await next();
    }

    /// <summary>
    /// Busca o valor de uma propriedade no request, suportando aninhamento em Request.
    /// </summary>
    private (object? value, bool found) GetPropertyValue(TRequest request, string propertyName)
    {
        var requestType = request.GetType();

        // Tentativa 1: Busca direto no Command
        var directProperty = requestType.GetProperty(propertyName);
        if (directProperty != null)
        {
            return (directProperty.GetValue(request), true);
        }

        // Tentativa 2: Busca dentro de propriedade "Request"
        var requestProperty = requestType.GetProperty("Request");
        if (requestProperty != null)
        {
            var requestObject = requestProperty.GetValue(request);
            if (requestObject != null)
            {
                var nestedProperty = requestObject.GetType().GetProperty(propertyName);
                if (nestedProperty != null)
                {
                    _logger.LogDebug(
                        "[UNIQUE-VALIDATION] Propriedade {PropertyName} encontrada em Request aninhado",
                        propertyName);
                    return (nestedProperty.GetValue(requestObject), true);
                }
            }
        }

        return (null, false);
    }

    private async Task ValidateUniqueConstraint(
        Type entityType,
        PropertyInfo property,
        UniqueAttribute attribute,
        object? propertyValue,
        object? entityId,
        Guid? tenantId,
        CancellationToken cancellationToken)
    {
        // Obtém o DbContext do módulo correto via DI
        var dbContextType = GetDbContextType(entityType);
        if (dbContextType == null)
        {
            _logger.LogWarning(
                "[UNIQUE-VALIDATION] DbContext não encontrado para {EntityType}",
                entityType.Name);
            return;
        }

        var dbContext = _serviceProvider.GetService(dbContextType) as DbContext;
        if (dbContext == null)
        {
            _logger.LogWarning(
                "[UNIQUE-VALIDATION] Não foi possível resolver DbContext {DbContextType}",
                dbContextType.Name);
            return;
        }

        // ✅ CORREÇÃO: Busca DbSet considerando proxies e herança
        var dbSetProperty = FindDbSetProperty(dbContext, entityType);
        if (dbSetProperty == null)
        {
            _logger.LogWarning(
                "[UNIQUE-VALIDATION] DbSet não encontrado para {EntityType} em {DbContextType}",
                entityType.Name,
                dbContextType.Name);
            return;
        }

        var dbSet = dbSetProperty.GetValue(dbContext) as IQueryable;
        if (dbSet == null)
        {
            _logger.LogWarning(
                "[UNIQUE-VALIDATION] Não foi possível obter IQueryable do DbSet para {EntityType}",
                entityType.Name);
            return;
        }

        // Constrói expressão: e => e.Property == propertyValue
        var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
        var propertyAccess = System.Linq.Expressions.Expression.Property(parameter, property);
        var constant = System.Linq.Expressions.Expression.Constant(propertyValue, property.PropertyType);
        var equals = System.Linq.Expressions.Expression.Equal(propertyAccess, constant);
        var lambda = System.Linq.Expressions.Expression.Lambda(equals, parameter);

        // Aplica filtro Where
        var whereMethod = typeof(Queryable).GetMethods()
            .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
            .MakeGenericMethod(entityType);

        var query = (IQueryable)whereMethod.Invoke(null, new object[] { dbSet, lambda })!;

        // Filtro por TenantId se scope = Tenant
        if (attribute.Scope == UniqueScope.Tenant && tenantId.HasValue)
        {
            var tenantIdProperty = entityType.GetProperty("TenantId");
            if (tenantIdProperty != null)
            {
                var tenantIdAccess = System.Linq.Expressions.Expression.Property(parameter, tenantIdProperty);
                var tenantIdConstant = System.Linq.Expressions.Expression.Constant(tenantId.Value, typeof(Guid));
                var tenantIdEquals = System.Linq.Expressions.Expression.Equal(tenantIdAccess, tenantIdConstant);
                var tenantIdLambda = System.Linq.Expressions.Expression.Lambda(tenantIdEquals, parameter);

                query = (IQueryable)whereMethod.Invoke(null, new object[] { query, tenantIdLambda })!;
            }
        }

        // Ignora o próprio registro em caso de edição
        if (entityId != null)
        {
            var idProperty = entityType.GetProperty("Id");
            if (idProperty != null)
            {
                var idAccess = System.Linq.Expressions.Expression.Property(parameter, idProperty);
                var idConstant = System.Linq.Expressions.Expression.Constant(entityId, idProperty.PropertyType);
                var idNotEquals = System.Linq.Expressions.Expression.NotEqual(idAccess, idConstant);
                var idLambda = System.Linq.Expressions.Expression.Lambda(idNotEquals, parameter);

                query = (IQueryable)whereMethod.Invoke(null, new object[] { query, idLambda })!;
            }
        }

        // Verifica se existe (usando AnyAsync)
        var anyAsyncMethod = typeof(EntityFrameworkQueryableExtensions).GetMethods()
            .First(m => m.Name == "AnyAsync" && m.GetParameters().Length == 2)
            .MakeGenericMethod(entityType);

        var existsTask = anyAsyncMethod.Invoke(null, new object[] { query, cancellationToken }) as Task<bool>;
        var exists = existsTask != null && await existsTask;

        if (exists)
        {
            var displayName = attribute.DisplayName ?? property.Name;
            var errorMessage = attribute.ErrorMessage ??
                $"{displayName} '{propertyValue}' já está cadastrado.";

            _logger.LogWarning(
                "[UNIQUE-VALIDATION] Duplicata detectada | Entity: {EntityType} | Property: {PropertyName} | Value: {PropertyValue}",
                entityType.Name, property.Name, propertyValue);

            throw new DuplicateEntityException(
                entityType.Name,
                property.Name,
                propertyValue,
                errorMessage);
        }

        _logger.LogDebug(
            "[UNIQUE-VALIDATION] ✓ Validação OK | Property: {PropertyName} | Value: {PropertyValue}",
            property.Name, propertyValue);
    }

    /// <summary>
    /// ✅ VERSÃO FINAL CORRIGIDA: Busca DbSet considerando proxies e herança.
    /// CRÍTICO: Não usa BindingFlags.DeclaredOnly para pegar propriedades herdadas!
    /// </summary>
    private PropertyInfo? FindDbSetProperty(DbContext dbContext, Type entityType)
    {
        // Pega o tipo real do DbContext (pode ser proxy)
        var dbContextType = dbContext.GetType();

        // Se for proxy, tenta usar BaseType
        if (dbContextType.BaseType != null &&
            dbContextType.BaseType != typeof(object) &&
            dbContextType.BaseType != typeof(DbContext))
        {
            dbContextType = dbContextType.BaseType;
            _logger.LogDebug(
                "[UNIQUE-VALIDATION] Detectado proxy, usando BaseType: {DbContextType}",
                dbContextType.Name);
        }

        _logger.LogDebug(
            "[UNIQUE-VALIDATION] Buscando DbSet para {EntityType} em {DbContextType}",
            entityType.Name, dbContextType.Name);

        // ✅ CRÍTICO: NÃO usar BindingFlags.DeclaredOnly
        // Isso garante que pegamos propriedades herdadas também!
        var properties = dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // Método 1: Busca por DbSet<TEntity> exato
        var exactMatch = properties.FirstOrDefault(p =>
            p.PropertyType.IsGenericType &&
            p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
            p.PropertyType.GetGenericArguments()[0] == entityType);

        if (exactMatch != null)
        {
            _logger.LogDebug(
                "[UNIQUE-VALIDATION] ✓ DbSet encontrado (exact match): {PropertyName}",
                exactMatch.Name);
            return exactMatch;
        }

        // Método 2: Busca por nome da entidade
        var byName = properties.FirstOrDefault(p =>
            p.PropertyType.IsGenericType &&
            p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
            p.Name.Equals(entityType.Name, StringComparison.OrdinalIgnoreCase));

        if (byName != null)
        {
            _logger.LogDebug(
                "[UNIQUE-VALIDATION] ✓ DbSet encontrado (by name): {PropertyName}",
                byName.Name);
            return byName;
        }

        // Método 3: Busca por nome plural (ex: CapVisitantes -> CapVisitanteses)
        var pluralName = entityType.Name + "es";
        var byPluralName = properties.FirstOrDefault(p =>
            p.PropertyType.IsGenericType &&
            p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
            p.Name.Equals(pluralName, StringComparison.OrdinalIgnoreCase));

        if (byPluralName != null)
        {
            _logger.LogDebug(
                "[UNIQUE-VALIDATION] ✓ DbSet encontrado (by plural): {PropertyName}",
                byPluralName.Name);
            return byPluralName;
        }

        // DEBUG: Lista todos os DbSets disponíveis
        var allDbSets = properties
            .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .Select(p => $"{p.Name} ({p.PropertyType.GetGenericArguments()[0].Name})")
            .ToList();

        _logger.LogWarning(
            "[UNIQUE-VALIDATION] DbSet não encontrado para {EntityType}. DbSets disponíveis em {DbContextType}: {DbSets}",
            entityType.Name,
            dbContextType.Name,
            string.Join(", ", allDbSets));

        return null;
    }

    private Type? GetDbContextType(Type entityType)
    {
        var entityNamespace = entityType.Namespace;
        if (string.IsNullOrEmpty(entityNamespace))
            return null;

        // Extrai o nome do módulo
        // Ex: RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities
        var parts = entityNamespace.Split('.');
        if (parts.Length < 3)
            return null;

        var moduleName = parts[2]; // GestaoTerceirosPrestadores

        // Tenta diferentes padrões de namespace do DbContext
        var possibleNames = new[]
        {
            $"RhSensoERP.Modules.{moduleName}.Infrastructure.Persistence.Contexts.{moduleName}DbContext",
            $"RhSensoERP.Modules.{moduleName}.Infrastructure.Persistence.{moduleName}DbContext",
            $"RhSensoERP.Modules.{moduleName}.Infrastructure.{moduleName}DbContext"
        };

        // Busca o tipo
        foreach (var typeName in possibleNames)
        {
            var dbContextType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.FullName == typeName);

            if (dbContextType != null)
            {
                _logger.LogDebug(
                    "[UNIQUE-VALIDATION] DbContext encontrado: {DbContextType}",
                    dbContextType.Name);
                return dbContextType;
            }
        }

        _logger.LogWarning(
            "[UNIQUE-VALIDATION] DbContext não encontrado. Tentativas: {Attempts}",
            string.Join(", ", possibleNames));

        return null;
    }
}