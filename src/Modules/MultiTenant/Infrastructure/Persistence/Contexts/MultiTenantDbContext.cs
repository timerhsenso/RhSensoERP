using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.MultiTenant.Core.Entities;

namespace RhSensoERP.Modules.MultiTenant.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de MultiTenant.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class MultiTenantDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="MultiTenantDbContext"/>.
    /// </summary>
    public MultiTenantDbContext(DbContextOptions<MultiTenantDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Configura o modelo de entidades, aplicando as configurações de mapeamento.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
