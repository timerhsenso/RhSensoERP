using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.GestaoDocumental.Core.Entities;

namespace RhSensoERP.Modules.GestaoDocumental.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de GestaoDocumental.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class GestaoDocumentalDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="GestaoDocumentalDbContext"/>.
    /// </summary>
    public GestaoDocumentalDbContext(DbContextOptions<GestaoDocumentalDbContext> options)
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
