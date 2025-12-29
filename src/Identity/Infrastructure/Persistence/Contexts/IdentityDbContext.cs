using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;
using RhSensoERP.Identity.Core.Entities;
using RhSensoERP.Shared.Core.Abstractions;
using RhSensoERP.Shared.Core.Attributes;

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
    // DBSETS (Entidades do Módulo Identity)
    // =========================================================================

    /// <summary>
    /// Sistemas cadastrados.
    /// </summary>
    public DbSet<Tsistema> Sistemas => Set<Tsistema>();

    /// <summary>
    /// Funções/Permissões do sistema.
    /// </summary>
    public DbSet<Funcao> Funcoes => Set<Funcao>();

    /// <summary>
    /// Botões associados às funções.
    /// </summary>
    public DbSet<BotaoFuncao> BotoesFuncao => Set<BotaoFuncao>();

    /// <summary>
    /// Grupos de usuários para controle de acesso.
    /// </summary>
    public DbSet<GrupoDeUsuario> GruposDeUsuario => Set<GrupoDeUsuario>();

    /// <summary>
    /// Relacionamento entre Grupos e Funções.
    /// </summary>
    public DbSet<GrupoFuncao> GruposFuncoes => Set<GrupoFuncao>();

    /// <summary>
    /// Usuários do sistema.
    /// </summary>
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    /// <summary>
    /// Logs de auditoria de segurança.
    /// </summary>
    public DbSet<SecurityAuditLog> SecurityAuditLogs => Set<SecurityAuditLog>();

    // =========================================================================
    // CONFIGURAÇÃO DO MODELO
    // =========================================================================

    /// <summary>
    /// Configura o modelo de entidades, aplicando as configurações de mapeamento.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =====================================================================
        // 1. Aplica configurações de IEntityTypeConfiguration automaticamente
        // =====================================================================
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // =====================================================================
        // 2. ⭐ Aplica configuração automática de triggers
        // =====================================================================
        ApplyDatabaseTriggersConfiguration(modelBuilder);
    }

    /// <summary>
    /// Aplica automaticamente UseSqlOutputClause(false) para entidades 
    /// marcadas com [HasDatabaseTriggers].
    /// 
    /// PROBLEMA: SQL Server não permite OUTPUT INSERTED/DELETED em tabelas com triggers AFTER.
    /// SOLUÇÃO: EF Core desabilita OUTPUT e faz SELECT separado após INSERT/UPDATE.
    /// </summary>
    private void ApplyDatabaseTriggersConfiguration(ModelBuilder modelBuilder)
    {
        var entitiesWithTriggers = new List<string>();

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            // Verifica se a entidade possui o atributo [HasDatabaseTriggers]
            var triggerAttribute = clrType
                .GetCustomAttributes(typeof(HasDatabaseTriggersAttribute), inherit: true)
                .FirstOrDefault() as HasDatabaseTriggersAttribute;

            if (triggerAttribute != null)
            {
                // ⭐ CORREÇÃO: Usar a API correta do EF Core
                // Configura a entidade para não usar OUTPUT CLAUSE
                entityType.SetAnnotation("SqlServer:UseSqlOutputClause", false);

                // Log para debug/documentação
                entitiesWithTriggers.Add(clrType.Name);

                Console.WriteLine(
                    $"[EF] 🔧 Triggers detectados em {clrType.Name} " +
                    $"| Descrição: {triggerAttribute.Description} " +
                    $"| OUTPUT CLAUSE desabilitado"
                );
            }
        }

        // Log consolidado
        if (entitiesWithTriggers.Any())
        {
            Console.WriteLine(
                $"[EF] ✅ Total de {entitiesWithTriggers.Count} entidade(s) " +
                $"com triggers configuradas: {string.Join(", ", entitiesWithTriggers)}"
            );
        }
        else
        {
            Console.WriteLine("[EF] ℹ️ Nenhuma entidade com triggers detectada no módulo Identity");
        }
    }

    // =========================================================================
    // IUNITOFWORK IMPLEMENTATION
    // =========================================================================

    /// <summary>
    /// Salva as alterações no banco de dados de forma assíncrona.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Número de registros afetados.</returns>
    async Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken ct)
    {
        return await base.SaveChangesAsync(ct);
    }
}