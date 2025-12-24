using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de GestaoTerceirosPrestadores.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class GestaoTerceirosPrestadoresDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="GestaoTerceirosPrestadoresDbContext"/>.
    /// </summary>
    public GestaoTerceirosPrestadoresDbContext(DbContextOptions<GestaoTerceirosPrestadoresDbContext> options)
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
