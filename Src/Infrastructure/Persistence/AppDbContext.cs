using Microsoft.EntityFrameworkCore;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Infrastructure.Persistence.Configurations.Security;
using RhSensoERP.Infrastructure.Persistence.Interceptors;

namespace RhSensoERP.Infrastructure.Persistence;

/// <summary>
/// Contexto principal do Entity Framework Core
/// Mapeia todas as entidades do sistema (legacy + SaaS)
/// </summary>
public class AppDbContext : DbContext
{
    // Construtor principal para uso em produção
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Construtor para testes com interceptor de auditoria
    public AppDbContext(DbContextOptions<AppDbContext> options, AuditSaveChangesInterceptor interceptor)
        : base(options)
    {
        // Adiciona o interceptor se fornecido nos testes
    }

    // ========================================
    // ENTIDADES LEGACY (Sistema OnPrem)
    // ========================================

    /// <summary>
    /// Usuários legacy (tabela tuse1)
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Sistemas (tabela tsistema)
    /// </summary>
    public DbSet<Sistema> Sistemas => Set<Sistema>();

    /// <summary>
    /// Funções (tabela fucn1)
    /// </summary>
    public DbSet<Funcao> Funcoes => Set<Funcao>();

    /// <summary>
    /// Botões de função (tabela btfuncao)
    /// </summary>
    public DbSet<BotaoFuncao> BotoesFuncao => Set<BotaoFuncao>();

    /// <summary>
    /// Grupos de usuário (tabela gurh1)
    /// </summary>
    public DbSet<GrupoDeUsuario> GruposDeUsuario => Set<GrupoDeUsuario>();

    /// <summary>
    /// Relacionamento usuário-grupo (tabela usrh1)
    /// </summary>
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();

    /// <summary>
    /// Permissões grupo-função (tabela hbrh1)
    /// </summary>
    public DbSet<GrupoFuncao> GruposFuncoes => Set<GrupoFuncao>();

    // ========================================
    // ENTIDADES SAAS (Sistema Multi-tenant)
    // ========================================

    /// <summary>
    /// Tenants SaaS
    /// </summary>
    public DbSet<SaasTenant> SaasTenants => Set<SaasTenant>();

    /// <summary>
    /// Usuários SaaS
    /// </summary>
    public DbSet<SaasUser> SaasUsers => Set<SaasUser>();

    /// <summary>
    /// Convites SaaS
    /// </summary>
    public DbSet<SaasInvitation> SaasInvitations => Set<SaasInvitation>();

    // ========================================
    // ENTIDADES FUTURAS (Sistema Unificado)
    // ========================================

    /// <summary>
    /// Grupos unificados (futuro)
    /// </summary>
    public DbSet<Group> Groups => Set<Group>();

    /// <summary>
    /// Roles unificados (futuro)
    /// </summary>
    public DbSet<Role> Roles => Set<Role>();

    /// <summary>
    /// Relacionamento grupo-role (futuro)
    /// </summary>
    public DbSet<GroupRole> GroupRoles => Set<GroupRole>();

    // ========================================
    // CONFIGURAÇÃO DO MODELO
    // ========================================

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========================================
        // CONFIGURAÇÕES LEGACY
        // ========================================

        modelBuilder.ApplyConfiguration(new UserConfig());
        modelBuilder.ApplyConfiguration(new SistemaConfig());
        modelBuilder.ApplyConfiguration(new FuncaoConfig());
        modelBuilder.ApplyConfiguration(new BotaoFuncaoConfig());
        modelBuilder.ApplyConfiguration(new GrupoDeUsuarioConfig());
        modelBuilder.ApplyConfiguration(new UserGroupConfig());
        modelBuilder.ApplyConfiguration(new GrupoFuncaoConfig());

        // ========================================
        // CONFIGURAÇÕES SAAS
        // ========================================

        modelBuilder.ApplyConfiguration(new SaasTenantConfig());
        modelBuilder.ApplyConfiguration(new SaasUserConfig());
        modelBuilder.ApplyConfiguration(new SaasInvitationConfig());

        // ========================================
        // FILTROS GLOBAIS
        // ========================================

        // Filtro global para soft delete nas entidades SaaS
        modelBuilder.Entity<SaasUser>().HasQueryFilter(x => x.IsActive);

        // Filtros para entidades legacy ativas
        modelBuilder.Entity<User>().HasQueryFilter(x => x.FlAtivo == 'S');
        modelBuilder.Entity<Sistema>().HasQueryFilter(x => x.Ativo);
    }

    // ========================================
    // QUERY FILTERS (EXTENSION METHODS)
    // ========================================

    /// <summary>
    /// Extensão para incluir entidades inativas na consulta
    /// </summary>
    public IQueryable<T> IncludeInactive<T>() where T : class
    {
        return Set<T>().IgnoreQueryFilters();
    }
}

/// <summary>
/// Extensões para facilitar consultas comuns
/// </summary>
public static class AppDbContextExtensions
{
    /// <summary>
    /// Filtro para usuários ativos
    /// </summary>
    public static IQueryable<User> Ativos(this DbSet<User> users)
    {
        return users.Where(u => u.FlAtivo == 'S');
    }

    /// <summary>
    /// Filtro para sistemas ativos
    /// </summary>
    public static IQueryable<Sistema> Ativos(this DbSet<Sistema> sistemas)
    {
        return sistemas.Where(s => s.Ativo);
    }

    /// <summary>
    /// Filtro para usuários SaaS por tenant
    /// </summary>
    public static IQueryable<SaasUser> ByTenant(this DbSet<SaasUser> users, Guid tenantId)
    {
        return users.Where(u => u.TenantId == tenantId);
    }

    /// <summary>
    /// Filtro para grupos de usuário ativos (sem data fim)
    /// </summary>
    public static IQueryable<UserGroup> Ativos(this DbSet<UserGroup> userGroups)
    {
        return userGroups.Where(ug => ug.DtFimVal == null);
    }

    /// <summary>
    /// Filtro para grupos ativos de um usuário específico
    /// </summary>
    public static IQueryable<UserGroup> AtivosDoUsuario(this DbSet<UserGroup> userGroups, string cdUsuario)
    {
        return userGroups.Where(ug => ug.CdUsuario == cdUsuario && ug.DtFimVal == null);
    }

    /// <summary>
    /// Filtro para convites SaaS válidos (não aceitos e não expirados)
    /// </summary>
    public static IQueryable<SaasInvitation> Validos(this DbSet<SaasInvitation> invitations)
    {
        return invitations.Where(i => !i.IsAccepted && i.ExpiresAt > DateTime.UtcNow);
    }
}