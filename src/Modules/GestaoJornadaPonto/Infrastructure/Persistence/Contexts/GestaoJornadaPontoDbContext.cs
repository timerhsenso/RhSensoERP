using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.GestaoJornadaPonto.Core.Entities;

namespace RhSensoERP.Modules.GestaoJornadaPonto.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de GestaoJornadaPonto.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class GestaoJornadaPontoDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="GestaoJornadaPontoDbContext"/>.
    /// </summary>
    public GestaoJornadaPontoDbContext(DbContextOptions<GestaoJornadaPontoDbContext> options)
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
