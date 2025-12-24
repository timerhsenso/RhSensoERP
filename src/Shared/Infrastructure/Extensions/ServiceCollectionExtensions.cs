// =============================================================================
// RHSENSOERP - SHARED INFRASTRUCTURE
// =============================================================================
// Arquivo: src/Shared/RhSensoERP.Shared.Infrastructure/Extensions/ServiceCollectionExtensions.cs
// Descrição: Extensões para registro de serviços genéricos no DI
// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RhSensoERP.Shared.Core.Abstractions;
using RhSensoERP.Shared.Infrastructure.Persistence;

namespace RhSensoERP.Shared.Infrastructure.Extensions;

/// <summary>
/// Extensões para IServiceCollection.
/// </summary>
public static class SharedServiceCollectionExtensions
{
    /// <summary>
    /// Adiciona o UnitOfWork ao container de DI.
    /// Registra tanto IUnitOfWork quanto IUnitOfWorkWithTransaction.
    /// </summary>
    /// <typeparam name="TContext">Tipo do DbContext</typeparam>
    public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.TryAddScoped<IUnitOfWork, UnitOfWork<TContext>>();
        services.TryAddScoped<IUnitOfWorkWithTransaction, UnitOfWork<TContext>>();
        services.TryAddScoped<UnitOfWork<TContext>>();
        return services;
    }

    /// <summary>
    /// Adiciona um repositório genérico com chave tipada ao container de DI.
    /// </summary>
    /// <typeparam name="TEntity">Tipo da entidade</typeparam>
    /// <typeparam name="TKey">Tipo da chave primária</typeparam>
    /// <typeparam name="TContext">Tipo do DbContext</typeparam>
    public static IServiceCollection AddRepositoryWithKey<TEntity, TKey, TContext>(this IServiceCollection services)
        where TEntity : class
        where TKey : notnull
        where TContext : DbContext
    {
        services.TryAddScoped<IRepositoryWithKey<TEntity, TKey>, GenericRepository<TEntity, TKey, TContext>>();
        services.TryAddScoped<IReadRepositoryWithKey<TEntity, TKey>, GenericRepository<TEntity, TKey, TContext>>();
        services.TryAddScoped<IWriteRepositoryWithKey<TEntity, TKey>, GenericRepository<TEntity, TKey, TContext>>();
        return services;
    }

    /// <summary>
    /// Adiciona um repositório customizado ao container de DI.
    /// </summary>
    /// <typeparam name="TInterface">Interface do repositório</typeparam>
    /// <typeparam name="TImplementation">Implementação do repositório</typeparam>
    public static IServiceCollection AddCustomRepository<TInterface, TImplementation>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        services.TryAddScoped<TInterface, TImplementation>();
        return services;
    }

    /// <summary>
    /// Adiciona um serviço CRUD ao container de DI.
    /// </summary>
    /// <typeparam name="TInterface">Interface do serviço</typeparam>
    /// <typeparam name="TImplementation">Implementação do serviço</typeparam>
    public static IServiceCollection AddCrudService<TInterface, TImplementation>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        services.TryAddScoped<TInterface, TImplementation>();
        return services;
    }

    /// <summary>
    /// Adiciona a infraestrutura completa para uma entidade (Repository + Service).
    /// </summary>
    public static IServiceCollection AddEntityInfrastructure<TEntity, TKey, TContext, TService, TServiceImpl>(
        this IServiceCollection services)
        where TEntity : class
        where TKey : notnull
        where TContext : DbContext
        where TService : class
        where TServiceImpl : class, TService
    {
        services.AddRepositoryWithKey<TEntity, TKey, TContext>();
        services.AddCrudService<TService, TServiceImpl>();
        return services;
    }
}

/// <summary>
/// Builder fluente para configuração de entidades.
/// </summary>
/// <typeparam name="TContext">Tipo do DbContext</typeparam>
public class EntityConfigBuilder<TContext>
    where TContext : DbContext
{
    private readonly IServiceCollection _services;

    public EntityConfigBuilder(IServiceCollection services)
    {
        _services = services;
    }

    /// <summary>
    /// Configura uma entidade com repositório e serviço.
    /// </summary>
    public EntityConfigBuilder<TContext> AddEntity<TEntity, TKey, TService, TServiceImpl>()
        where TEntity : class
        where TKey : notnull
        where TService : class
        where TServiceImpl : class, TService
    {
        _services.AddEntityInfrastructure<TEntity, TKey, TContext, TService, TServiceImpl>();
        return this;
    }

    /// <summary>
    /// Configura uma entidade apenas com repositório.
    /// </summary>
    public EntityConfigBuilder<TContext> AddEntity<TEntity, TKey>()
        where TEntity : class
        where TKey : notnull
    {
        _services.AddRepositoryWithKey<TEntity, TKey, TContext>();
        return this;
    }

    /// <summary>
    /// Configura um repositório customizado.
    /// </summary>
    public EntityConfigBuilder<TContext> AddCustomRepository<TInterface, TImplementation>()
        where TInterface : class
        where TImplementation : class, TInterface
    {
        _services.AddCustomRepository<TInterface, TImplementation>();
        return this;
    }

    /// <summary>
    /// Retorna o IServiceCollection para encadeamento.
    /// </summary>
    public IServiceCollection Build() => _services;
}

/// <summary>
/// Extensões para iniciar o builder de entidades.
/// </summary>
public static class EntityConfigBuilderExtensions
{
    /// <summary>
    /// Inicia a configuração de entidades para um DbContext.
    /// </summary>
    /// <typeparam name="TContext">Tipo do DbContext</typeparam>
    public static EntityConfigBuilder<TContext> ConfigureEntitiesWithKey<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddUnitOfWork<TContext>();
        return new EntityConfigBuilder<TContext>(services);
    }
}
