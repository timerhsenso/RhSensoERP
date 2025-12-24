using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.GestaoTalentosDesempenho.Core.Entities;

namespace RhSensoERP.Modules.GestaoTalentosDesempenho.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de GestaoTalentosDesempenho.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class GestaoTalentosDesempenhoDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="GestaoTalentosDesempenhoDbContext"/>.
    /// </summary>
    public GestaoTalentosDesempenhoDbContext(DbContextOptions<GestaoTalentosDesempenhoDbContext> options)
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
