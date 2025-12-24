using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.SaudeSegurancaTrabalho.Core.Entities;

namespace RhSensoERP.Modules.SaudeSegurancaTrabalho.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de SaudeSegurancaTrabalho.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class SaudeSegurancaTrabalhoDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="SaudeSegurancaTrabalhoDbContext"/>.
    /// </summary>
    public SaudeSegurancaTrabalhoDbContext(DbContextOptions<SaudeSegurancaTrabalhoDbContext> options)
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
