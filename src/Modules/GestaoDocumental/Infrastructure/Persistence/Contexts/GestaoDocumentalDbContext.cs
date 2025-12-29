using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;
using RhSensoERP.Modules.GestaoDocumental.Core.Entities;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDocumental.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo de GestaoDocumental.
///
/// Responsável por expor os conjuntos de entidades do módulo e
/// aplicar as configurações de mapeamento (IEntityTypeConfiguration).
/// </summary>
public class GestaoDocumentalDbContext : DbContext
{
    /// <summary>
    /// Construtor padrão do <see cref="GestaoDocumentalDbContext"/>.
    /// </summary>
    public GestaoDocumentalDbContext(DbContextOptions<GestaoDocumentalDbContext> options)
        : base(options)
    {
    }

    // =========================================================================
    // DBSETS (Entidades do Módulo)
    // =========================================================================

    // Adicione seus DbSets conforme necessário

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
            Console.WriteLine("[EF] ℹ️ Nenhuma entidade com triggers detectada");
        }
    }
}