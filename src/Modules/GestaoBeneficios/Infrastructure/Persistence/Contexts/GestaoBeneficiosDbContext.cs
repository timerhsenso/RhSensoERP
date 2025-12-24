using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.GestaoBeneficios.Core.Entities;

namespace RhSensoERP.Modules.GestaoBeneficios.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de GestaoBeneficios.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class GestaoBeneficiosDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="GestaoBeneficiosDbContext"/>.
    /// </summary>
    public GestaoBeneficiosDbContext(DbContextOptions<GestaoBeneficiosDbContext> options)
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
