using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.RecrutamentoSelecao.Core.Entities;

namespace RhSensoERP.Modules.RecrutamentoSelecao.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de RecrutamentoSelecao.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class RecrutamentoSelecaoDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="RecrutamentoSelecaoDbContext"/>.
    /// </summary>
    public RecrutamentoSelecaoDbContext(DbContextOptions<RecrutamentoSelecaoDbContext> options)
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
