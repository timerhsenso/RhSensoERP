using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.CargosSalariosRemuneracao.Core.Entities;

namespace RhSensoERP.Modules.CargosSalariosRemuneracao.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de CargosSalariosRemuneracao.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class CargosSalariosRemuneracaoDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="CargosSalariosRemuneracaoDbContext"/>.
    /// </summary>
    public CargosSalariosRemuneracaoDbContext(DbContextOptions<CargosSalariosRemuneracaoDbContext> options)
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
