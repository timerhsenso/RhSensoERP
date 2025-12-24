using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.TreinamentoDesenvolvimento.Core.Entities;

namespace RhSensoERP.Modules.TreinamentoDesenvolvimento.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de TreinamentoDesenvolvimento.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class TreinamentoDesenvolvimentoDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="TreinamentoDesenvolvimentoDbContext"/>.
    /// </summary>
    public TreinamentoDesenvolvimentoDbContext(DbContextOptions<TreinamentoDesenvolvimentoDbContext> options)
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
