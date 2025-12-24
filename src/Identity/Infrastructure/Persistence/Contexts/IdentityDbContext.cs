using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Identity.Core.Entities;
using RhSensoERP.Shared.Core.Abstractions;

namespace RhSensoERP.Identity.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo Identity.
/// 
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public sealed class IdentityDbContext : DbContext, IUnitOfWork
{
    /// <summary>
    /// Construtor padrão do <see cref="IdentityDbContext"/>.
    /// </summary>
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    // =========================================================================
    // DbSets
    // =========================================================================
    public DbSet<Tsistema> Sistemas => Set<Tsistema>();
    public DbSet<Funcao> Funcoes => Set<Funcao>();
    public DbSet<BotaoFuncao> BotoesFuncao => Set<BotaoFuncao>();
    public DbSet<GrupoDeUsuario> GruposDeUsuario => Set<GrupoDeUsuario>();
    public DbSet<GrupoFuncao> GruposFuncoes => Set<GrupoFuncao>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<SecurityAuditLog> SecurityAuditLogs => Set<SecurityAuditLog>();

    // =========================================================================
    // Configuração
    // =========================================================================

    /// <summary>
    /// Configura o modelo de entidades, aplicando as configurações de mapeamento.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    // =========================================================================
    // IUnitOfWork
    // =========================================================================

    async Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken ct)
    {
        return await base.SaveChangesAsync(ct);
    }
}