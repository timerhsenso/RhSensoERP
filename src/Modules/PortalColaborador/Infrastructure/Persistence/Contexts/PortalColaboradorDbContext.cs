using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.PortalColaborador.Core.Entities;

namespace RhSensoERP.Modules.PortalColaborador.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de PortalColaborador.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class PortalColaboradorDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="PortalColaboradorDbContext"/>.
    /// </summary>
    public PortalColaboradorDbContext(DbContextOptions<PortalColaboradorDbContext> options)
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
