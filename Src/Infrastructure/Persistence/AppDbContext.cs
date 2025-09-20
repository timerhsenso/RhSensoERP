// ==================================================================================
// APPDBCONTEXT COMPLETO ATUALIZADO
// ==================================================================================
// Arquivo: Src/Infrastructure/Persistence/AppDbContext.cs
// Descrição: Contexto principal do Entity Framework Core para o sistema RhSensoERP
// Mapeia todas as entidades de segurança para as tabelas legacy do banco bd_rhu_copenor
// ==================================================================================

using Microsoft.EntityFrameworkCore;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Infrastructure.Persistence.Configurations.Security;
using RhSensoERP.Infrastructure.Persistence.Interceptors;

namespace RhSensoERP.Infrastructure.Persistence;

/// <summary>
/// Contexto principal do Entity Framework Core para o sistema RhSensoERP
/// 
/// Responsabilidades:
/// - Mapear entidades de domínio para tabelas legacy
/// - Configurar relacionamentos entre entidades
/// - Aplicar interceptors de auditoria
/// - Fornecer DbSets para acesso aos dados
/// 
/// Padrões implementados:
/// - Repository pattern via DbSet
/// - Unit of Work via SaveChanges
/// - Audit interceptor para rastreamento de mudanças
/// </summary>
public class AppDbContext : DbContext
{
    #region Private Fields

    private readonly AuditSaveChangesInterceptor _auditInterceptor;

    #endregion

    #region Constructor

    /// <summary>
    /// Construtor do contexto com injeção de dependências
    /// </summary>
    /// <param name="options">Opções de configuração do EF Core</param>
    /// <param name="auditInterceptor">Interceptor para auditoria automática</param>
    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        AuditSaveChangesInterceptor auditInterceptor) : base(options)
    {
        _auditInterceptor = auditInterceptor;
    }

    #endregion

    #region DbSets - Módulo Security (SEG)

    /// <summary>
    /// Usuários do sistema
    /// Tabela: tuse1
    /// Configuração: UserConfig.cs
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Sistemas disponíveis no ERP (SEG, RHU, FIN, etc.)
    /// Tabela: tsistema
    /// Configuração: SistemaConfig.cs
    /// </summary>
    public DbSet<Sistema> Sistemas => Set<Sistema>();

    /// <summary>
    /// Funções/telas de cada sistema
    /// Tabela: fucn1
    /// Configuração: FuncaoConfig.cs
    /// Chave: (cdsistema, cdfuncao)
    /// </summary>
    public DbSet<Funcao> Funcoes => Set<Funcao>();

    /// <summary>
    /// Botões/ações dentro das funções
    /// Tabela: btfuncao
    /// Configuração: BotaoFuncaoConfig.cs
    /// Chave: (cdsistema, cdfuncao, nmbotao)
    /// </summary>
    public DbSet<BotaoFuncao> BotoesFuncao => Set<BotaoFuncao>();

    /// <summary>
    /// Relacionamento usuário-grupo com controle temporal
    /// Tabela: usrh1
    /// Configuração: UserGroupConfig.cs
    /// Controla período de validade (dtinival, dtfimval)
    /// </summary>
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();

    /// <summary>
    /// Grupos de usuários por sistema
    /// Tabela: gurh1
    /// Configuração: GrupoDeUsuarioConfig.cs
    /// Chave: (cdsistema, cdgruser)
    /// </summary>
    public DbSet<GrupoDeUsuario> GruposDeUsuario => Set<GrupoDeUsuario>();

    /// <summary>
    /// Permissões dos grupos nas funções
    /// Tabela: hbrh1
    /// Configuração: GrupoFuncaoConfig.cs
    /// Define ações permitidas (cdacoes) e restrições (cdrestric)
    /// </summary>
    public DbSet<GrupoFuncao> GruposFuncoes => Set<GrupoFuncao>();

    #endregion

    #region DbSets - Entidades Modernas (Opcionais)

    /// <summary>
    /// Grupos modernos - sistema novo de grupos (opcional)
    /// Usar apenas se implementar sistema moderno paralelo ao legacy
    /// </summary>
    public DbSet<Group> Groups => Set<Group>();

    /// <summary>
    /// Roles modernos - sistema novo de papéis (opcional)
    /// Usar apenas se implementar sistema moderno paralelo ao legacy
    /// </summary>
    public DbSet<Role> Roles => Set<Role>();

    /// <summary>
    /// Relacionamento grupo-role moderno (opcional)
    /// Usar apenas se implementar sistema moderno paralelo ao legacy
    /// </summary>
    public DbSet<GroupRole> GroupRoles => Set<GroupRole>();

    #endregion

    #region EF Core Configuration

    /// <summary>
    /// Configuração de interceptors e comportamentos do EF Core
    /// </summary>
    /// <param name="optionsBuilder">Builder de opções</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Adiciona interceptor de auditoria para rastreamento automático
        // de criação/atualização de registros
        optionsBuilder.AddInterceptors(_auditInterceptor);

        base.OnConfiguring(optionsBuilder);
    }

    /// <summary>
    /// Configuração do modelo de dados - mapeamentos e relacionamentos
    /// </summary>
    /// <param name="modelBuilder">Builder do modelo</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ==================================================================================
        // APLICAÇÃO DAS CONFIGURAÇÕES DE ENTIDADES
        // ==================================================================================

        // Aplica configuração específica do User (já existente)
        modelBuilder.ApplyConfiguration(new UserConfig());

        // Aplica todas as novas configurações
        modelBuilder.ApplyConfiguration(new SistemaConfig());
        modelBuilder.ApplyConfiguration(new FuncaoConfig());
        modelBuilder.ApplyConfiguration(new BotaoFuncaoConfig());
        modelBuilder.ApplyConfiguration(new UserGroupConfig());
        modelBuilder.ApplyConfiguration(new GrupoDeUsuarioConfig());
        modelBuilder.ApplyConfiguration(new GrupoFuncaoConfig());

        // OU usar aplicação automática de todas as configurações:
        // modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfig).Assembly);

        // ==================================================================================
        // CONFIGURAÇÕES GLOBAIS
        // ==================================================================================

        ConfigureGlobalConventions(modelBuilder);
        ConfigureLegacyBehaviors(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Configurações globais aplicadas a todas as entidades
    /// </summary>
    /// <param name="modelBuilder">Builder do modelo</param>
    private static void ConfigureGlobalConventions(ModelBuilder modelBuilder)
    {
        // ==================================================================================
        // CONVENÇÕES DE NOMEAÇÃO
        // ==================================================================================

        // Remove pluralização automática de tabelas (se habilitada)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Preserva nomes de tabela configurados explicitamente
            if (entityType.GetTableName() == entityType.DisplayName())
            {
                entityType.SetTableName(entityType.DisplayName());
            }
        }

        // ==================================================================================
        // CONFIGURAÇÕES DE STRING
        // ==================================================================================

        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(string)))
        {
            // Define collation padrão para campos string (se necessário)
            // Descomentee caso o banco use collation específica
            // property.SetCollation("SQL_Latin1_General_CP1_CI_AS");

            // Configurações padrão para campos não configurados explicitamente
            if (property.GetMaxLength() == null && property.GetColumnType() == null)
            {
                property.SetMaxLength(255); // Tamanho padrão para strings
            }
        }
    }

    /// <summary>
    /// Configurações específicas para compatibilidade com sistema legacy
    /// </summary>
    /// <param name="modelBuilder">Builder do modelo</param>
    private static void ConfigureLegacyBehaviors(ModelBuilder modelBuilder)
    {
        // ==================================================================================
        // SISTEMA LEGACY - SEM SOFT DELETE AUTOMÁTICO
        // ==================================================================================

        // IMPORTANTE: O sistema legacy NÃO usa soft delete padrão (IsDeleted)
        // Usa campos específicos como FlAtivo = 'S'/'N' na tabela tuse1
        // Por isso NÃO aplicamos global query filters automáticos

        // Se fosse sistema moderno, usaríamos:
        // foreach (var entityType in modelBuilder.Model.GetEntityTypes()
        //     .Where(e => typeof(ISoftDeletable).IsAssignableFrom(e.ClrType)))
        // {
        //     modelBuilder.Entity(entityType.ClrType)
        //         .HasQueryFilter(CreateSoftDeleteFilter(entityType.ClrType));
        // }

        // ==================================================================================
        // CONFIGURAÇÕES DE PERFORMANCE
        // ==================================================================================

        // Configuração padrão de tracking para consultas
        // NoTracking por padrão já está configurado no Program.cs:
        // .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)

        // ==================================================================================
        // CONFIGURAÇÕES DE TIMEZONE (SE NECESSÁRIO)
        // ==================================================================================

        // Se o banco armazena datas em UTC, configurar conversão automática
        // foreach (var property in modelBuilder.Model.GetEntityTypes()
        //     .SelectMany(t => t.GetProperties())
        //     .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)))
        // {
        //     property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
        //         v => v.ToUniversalTime(),
        //         v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
        // }
    }

    #endregion
}

// ==================================================================================
// EXTENSÕES PARA FACILITAR QUERIES COMUNS
// ==================================================================================

/// <summary>
/// Extensões úteis para o AppDbContext
/// Facilitam queries comuns no sistema legacy
/// </summary>
public static class AppDbContextExtensions
{
    #region User Extensions

    /// <summary>
    /// Obtém usuários ativos (FlAtivo = 'S')
    /// </summary>
    /// <param name="users">DbSet de usuários</param>
    /// <returns>Query filtrada para usuários ativos</returns>
    public static IQueryable<User> Ativos(this DbSet<User> users)
    {
        return users.Where(u => u.FlAtivo == 'S');
    }

    /// <summary>
    /// Obtém usuários por tipo
    /// </summary>
    /// <param name="users">DbSet de usuários</param>
    /// <param name="tipo">Tipo do usuário (A/U/S/C)</param>
    /// <returns>Query filtrada por tipo</returns>
    public static IQueryable<User> PorTipo(this DbSet<User> users, char tipo)
    {
        return users.Where(u => u.TpUsuario == tipo);
    }

    /// <summary>
    /// Obtém usuários de uma empresa
    /// </summary>
    /// <param name="users">DbSet de usuários</param>
    /// <param name="cdEmpresa">Código da empresa</param>
    /// <returns>Query filtrada por empresa</returns>
    public static IQueryable<User> DaEmpresa(this DbSet<User> users, int cdEmpresa)
    {
        return users.Where(u => u.CdEmpresa == cdEmpresa);
    }

    #endregion

    #region Sistema Extensions

    /// <summary>
    /// Obtém sistemas ativos
    /// </summary>
    /// <param name="sistemas">DbSet de sistemas</param>
    /// <returns>Query filtrada para sistemas ativos</returns>
    public static IQueryable<Sistema> Ativos(this DbSet<Sistema> sistemas)
    {
        return sistemas.Where(s => s.Ativo);
    }

    #endregion

    #region UserGroup Extensions

    /// <summary>
    /// Obtém grupos ativos de um usuário (sem data fim)
    /// </summary>
    /// <param name="userGroups">DbSet de user groups</param>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <returns>Query filtrada para grupos ativos</returns>
    public static IQueryable<UserGroup> AtivosDoUsuario(this DbSet<UserGroup> userGroups, string cdUsuario)
    {
        return userGroups.Where(ug =>
            ug.CdUsuario == cdUsuario &&
            ug.DtFimVal == null);
    }

    /// <summary>
    /// Obtém grupos válidos em uma data específica
    /// </summary>
    /// <param name="userGroups">DbSet de user groups</param>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="data">Data de referência</param>
    /// <returns>Query filtrada para grupos válidos na data</returns>
    public static IQueryable<UserGroup> ValidosNaData(this DbSet<UserGroup> userGroups, string cdUsuario, DateTime data)
    {
        return userGroups.Where(ug =>
            ug.CdUsuario == cdUsuario &&
            ug.DtIniVal <= data &&
            (ug.DtFimVal == null || ug.DtFimVal >= data));
    }

    #endregion

    #region GrupoFuncao Extensions

    /// <summary>
    /// Obtém permissões de um grupo específico
    /// </summary>
    /// <param name="grupoFuncoes">DbSet de grupo funções</param>
    /// <param name="cdGrUser">Código do grupo</param>
    /// <returns>Query filtrada para permissões do grupo</returns>
    public static IQueryable<GrupoFuncao> DoGrupo(this DbSet<GrupoFuncao> grupoFuncoes, string cdGrUser)
    {
        return grupoFuncoes.Where(gf => gf.CdGrUser == cdGrUser);
    }

    /// <summary>
    /// Obtém permissões de um sistema específico
    /// </summary>
    /// <param name="grupoFuncoes">DbSet de grupo funções</param>
    /// <param name="cdSistema">Código do sistema</param>
    /// <returns>Query filtrada para permissões do sistema</returns>
    public static IQueryable<GrupoFuncao> DoSistema(this DbSet<GrupoFuncao> grupoFuncoes, string cdSistema)
    {
        return grupoFuncoes.Where(gf => gf.CdSistema == cdSistema);
    }

    /// <summary>
    /// Verifica se grupo tem acesso a uma função específica
    /// </summary>
    /// <param name="grupoFuncoes">DbSet de grupo funções</param>
    /// <param name="cdGrUser">Código do grupo</param>
    /// <param name="cdSistema">Código do sistema</param>
    /// <param name="cdFuncao">Código da função</param>
    /// <returns>Query para verificar acesso</returns>
    public static IQueryable<GrupoFuncao> ComAcessoA(this DbSet<GrupoFuncao> grupoFuncoes,
        string cdGrUser, string cdSistema, string cdFuncao)
    {
        return grupoFuncoes.Where(gf =>
            gf.CdGrUser == cdGrUser &&
            gf.CdSistema == cdSistema &&
            gf.CdFuncao == cdFuncao);
    }

    #endregion
}

// ==================================================================================
// CLASSE DE CONSTANTES PARA QUERIES COMUNS
// ==================================================================================

/// <summary>
/// Constantes úteis para o sistema legacy
/// </summary>
public static class LegacyConstants
{
    #region Status de Usuário

    public const char USUARIO_ATIVO = 'S';
    public const char USUARIO_INATIVO = 'N';

    #endregion

    #region Tipos de Usuário

    public const char TIPO_ADMINISTRADOR = 'A';
    public const char TIPO_USUARIO = 'U';
    public const char TIPO_SISTEMA = 'S';
    public const char TIPO_CONSULTA = 'C';

    #endregion

    #region Códigos de Ação

    public const char ACAO_INCLUIR = 'I';
    public const char ACAO_ALTERAR = 'A';
    public const char ACAO_EXCLUIR = 'E';
    public const char ACAO_CONSULTAR = 'C';

    public const string ACOES_COMPLETAS = "IAEC";
    public const string ACOES_LEITURA = "C";
    public const string ACOES_ESCRITA = "IAE";

    #endregion

    #region Códigos de Sistema

    public const string SISTEMA_SEGURANCA = "SEG";
    public const string SISTEMA_RH = "RHU";
    public const string SISTEMA_FINANCEIRO = "FIN";
    public const string SISTEMA_ESTOQUE = "EST";

    #endregion
}