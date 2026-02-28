using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Identity.Core.Entities;
using RhSensoERP.Shared.Core.Abstractions;
using RhSensoERP.Shared.Core.Attributes;

// ✅ NOVO: Referência ao módulo Segurança (entidades legadas)
using RhSensoERP.Modules.Seguranca.Core.Entities;
using RhSensoERP.Modules.Seguranca.Infrastructure.Persistence.Contexts;

namespace RhSensoERP.Identity.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo Identity.
///
/// Responsável por:
/// - Entidades de autenticação (Usuario, UserSecurity, RefreshToken, etc.)
/// - Carrega configurations do módulo Segurança para entidades legadas
///   (Tsistema, Funcao, BotaoFuncao, etc.) usadas nos joins de permissões.
/// - Configura o relacionamento cross-módulo Usuario ↔ UserGroup.
/// </summary>
public sealed class IdentityDbContext : DbContext, IUnitOfWork
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    // =========================================================================
    // DBSETS (Entidades de Autenticação — módulo Identity)
    // =========================================================================

    /// <summary>Usuários do sistema (tuse1).</summary>
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    /// <summary>Logs de auditoria de segurança.</summary>
    public DbSet<SecurityAuditLog> SecurityAuditLogs => Set<SecurityAuditLog>();

    // =========================================================================
    // ENTIDADES LEGADAS (via Set<T>() — sem DbSet explícito)
    // =========================================================================
    // As entidades Tsistema, Funcao, BotaoFuncao, GrupoDeUsuario, GrupoFuncao,
    // UserGroup são acessadas via _db.Set<T>() nos repositórios.
    // Suas configurations estão no assembly do módulo Segurança.
    // =========================================================================

    // =========================================================================
    // CONFIGURAÇÃO DO MODELO
    // =========================================================================

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =====================================================================
        // 1. Aplica configurations do próprio Identity
        //    (Usuario, UserSecurity, RefreshToken, LoginAuditLog, etc.)
        // =====================================================================
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // =====================================================================
        // 2. ✅ NOVO: Aplica configurations do módulo Segurança
        //    (Tsistema, Funcao, BotaoFuncao, GrupoDeUsuario, GrupoFuncao, UserGroup)
        //    Necessário para que os joins no PermissaoRepository funcionem.
        // =====================================================================
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SegurancaDbContext).Assembly);

        // =====================================================================
        // 3. ✅ NOVO: Override cross-módulo — Usuario ↔ UserGroup
        //    O UserGroupConfiguration no Segurança NÃO configura este
        //    relacionamento (para evitar dependência circular).
        //    Configuramos aqui porque Identity conhece ambas as entidades.
        // =====================================================================
        modelBuilder.Entity<UsuarioGrupo>(entity =>
        {
            entity.HasOne<Usuario>()
                .WithMany(u => u.UsuarioGrupos)
                .HasForeignKey(ug => ug.CdUsuario)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================================================================
        // 4. Aplica configuração automática de triggers
        // =====================================================================
        ApplyDatabaseTriggersConfiguration(modelBuilder);
    }

    /// <summary>
    /// Aplica UseSqlOutputClause(false) para entidades com [HasDatabaseTriggers].
    /// </summary>
    private void ApplyDatabaseTriggersConfiguration(ModelBuilder modelBuilder)
    {
        var entitiesWithTriggers = new List<string>();

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            var triggerAttribute = clrType
                .GetCustomAttributes(typeof(HasDatabaseTriggersAttribute), inherit: true)
                .FirstOrDefault() as HasDatabaseTriggersAttribute;

            if (triggerAttribute != null)
            {
                entityType.SetAnnotation("SqlServer:UseSqlOutputClause", false);
                entitiesWithTriggers.Add(clrType.Name);

                Console.WriteLine(
                    $"[EF] 🔧 Triggers detectados em {clrType.Name} " +
                    $"| Descrição: {triggerAttribute.Description} " +
                    $"| OUTPUT CLAUSE desabilitado");
            }
        }

        if (entitiesWithTriggers.Any())
        {
            Console.WriteLine(
                $"[EF] ✅ Total de {entitiesWithTriggers.Count} entidade(s) " +
                $"com triggers configuradas: {string.Join(", ", entitiesWithTriggers)}");
        }
        else
        {
            Console.WriteLine("[EF] ℹ️ Nenhuma entidade com triggers detectada no módulo Identity");
        }
    }

    // =========================================================================
    // IUNITOFWORK IMPLEMENTATION
    // =========================================================================

    async Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken ct)
    {
        return await base.SaveChangesAsync(ct);
    }
}