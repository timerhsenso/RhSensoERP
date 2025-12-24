using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

namespace RhSensoERP.Modules.AdministracaoPessoal.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de AdministracaoPessoal.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class AdministracaoPessoalDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="AdministracaoPessoalDbContext"/>.
    /// </summary>
    public AdministracaoPessoalDbContext(DbContextOptions<AdministracaoPessoalDbContext> options)
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
