using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.ComplianceTrabalhistaJuridico.Core.Entities;

namespace RhSensoERP.Modules.ComplianceTrabalhistaJuridico.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de ComplianceTrabalhistaJuridico.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class ComplianceTrabalhistaJuridicoDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="ComplianceTrabalhistaJuridicoDbContext"/>.
    /// </summary>
    public ComplianceTrabalhistaJuridicoDbContext(DbContextOptions<ComplianceTrabalhistaJuridicoDbContext> options)
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
