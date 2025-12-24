using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.ViagensDespesas.Core.Entities;

namespace RhSensoERP.Modules.ViagensDespesas.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de ViagensDespesas.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class ViagensDespesasDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="ViagensDespesasDbContext"/>.
    /// </summary>
    public ViagensDespesasDbContext(DbContextOptions<ViagensDespesasDbContext> options)
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
