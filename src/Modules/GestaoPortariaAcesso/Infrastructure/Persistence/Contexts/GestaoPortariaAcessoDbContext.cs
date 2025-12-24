using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.GestaoPortariaAcesso.Core.Entities;

namespace RhSensoERP.Modules.GestaoPortariaAcesso.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de GestaoPortariaAcesso.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class GestaoPortariaAcessoDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="GestaoPortariaAcessoDbContext"/>.
    /// </summary>
    public GestaoPortariaAcessoDbContext(DbContextOptions<GestaoPortariaAcessoDbContext> options)
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
