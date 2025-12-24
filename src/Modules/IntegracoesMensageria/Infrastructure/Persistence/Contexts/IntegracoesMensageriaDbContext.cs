using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.IntegracoesMensageria.Core.Entities;

namespace RhSensoERP.Modules.IntegracoesMensageria.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de IntegracoesMensageria.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class IntegracoesMensageriaDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="IntegracoesMensageriaDbContext"/>.
    /// </summary>
    public IntegracoesMensageriaDbContext(DbContextOptions<IntegracoesMensageriaDbContext> options)
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
