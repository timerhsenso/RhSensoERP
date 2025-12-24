using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.FolhaPagamentoEncargos.Core.Entities;

namespace RhSensoERP.Modules.FolhaPagamentoEncargos.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de FolhaPagamentoEncargos.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class FolhaPagamentoEncargosDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="FolhaPagamentoEncargosDbContext"/>.
    /// </summary>
    public FolhaPagamentoEncargosDbContext(DbContextOptions<FolhaPagamentoEncargosDbContext> options)
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
