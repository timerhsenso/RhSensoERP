using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.PeopleAnalyticsBI.Core.Entities;

namespace RhSensoERP.Modules.PeopleAnalyticsBI.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de PeopleAnalyticsBI.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class PeopleAnalyticsBIDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="PeopleAnalyticsBIDbContext"/>.
    /// </summary>
    public PeopleAnalyticsBIDbContext(DbContextOptions<PeopleAnalyticsBIDbContext> options)
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
