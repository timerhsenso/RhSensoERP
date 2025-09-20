// ==================================================================================
// APPDBCONTEXT COMPLETO ATUALIZADO
// ==================================================================================
// Arquivo: Src/Infrastructure/Persistence/AppDbContext.cs
// Descri��o: Contexto principal do Entity Framework Core para o sistema RhSensoERP
// Mapeia todas as entidades de seguran�a para as tabelas legacy do banco bd_rhu_copenor
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
/// - Mapear entidades de dom�nio para tabelas legacy
/// - Configurar relacionamentos entre entidades
/// - Aplicar interceptors de auditoria
/// - Fornecer DbSets para acesso aos dados
/// 
/// Padr�es implementados:
/// - Repository pattern via DbSet
/// - Unit of Work via SaveChanges
/// - Audit interceptor para rastreamento de mudan�as
/// </summary>
public class AppDbContext : DbContext
{
    #region Private Fields

    private readonly AuditSaveChangesInterceptor _auditInterceptor;

    #endregion

    #region Constructor

    /// <summary>
    /// Construtor do contexto com inje��o de depend�ncias
    /// </summary>
    /// <param name="options">Op��es de configura��o do EF Core</param>
    /// <param name="auditInterceptor">Interceptor para auditoria autom�tica</param>
    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        AuditSaveChangesInterceptor auditInterceptor) : base(options)
    {
        _auditInterceptor = auditInterceptor;
    }

    #endregion

    #region DbSets - M�dulo Security (SEG)

    /// <summary>
    /// Usu�rios do sistema
    /// Tabela: tuse1
    /// Configura��o: UserConfig.cs
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Sistemas dispon�veis no ERP (SEG, RHU, FIN, etc.)
    /// Tabela: tsistema
    /// Configura��o: SistemaConfig.cs
    /// </summary>
    public DbSet<Sistema> Sistemas => Set<Sistema>();

    /// <summary>
    /// Fun��es/telas de cada sistema
    /// Tabela: fucn1
    /// Configura��o: FuncaoConfig.cs
    /// Chave: (cdsistema, cdfuncao)
    /// </summary>
    public DbSet<Funcao> Funcoes => Set<Funcao>();

    /// <summary>
    /// Bot�es/a��es dentro das fun��es
    /// Tabela: btfuncao
    /// Configura��o: BotaoFuncaoConfig.cs
    /// Chave: (cdsistema, cdfuncao, nmbotao)
    /// </summary>
    public DbSet<BotaoFuncao> BotoesFuncao => Set<BotaoFuncao>();

    /// <summary>
    /// Relacionamento usu�rio-grupo com controle temporal
    /// Tabela: usrh1
    /// Configura��o: UserGroupConfig.cs
    /// Controla per�odo de validade (dtinival, dtfimval)
    /// </summary>
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();

    /// <summary>
    /// Grupos de usu�rios por sistema
    /// Tabela: gurh1
    /// Configura��o: GrupoDeUsuarioConfig.cs
    /// Chave: (cdsistema, cdgruser)
    /// </summary>
    public DbSet<GrupoDeUsuario> GruposDeUsuario => Set<GrupoDeUsuario>();

    /// <summary>
    /// Permiss�es dos grupos nas fun��es
    /// Tabela: hbrh1
    /// Configura��o: GrupoFuncaoConfig.cs
    /// Define a��es permitidas (cdacoes) e restri��es (cdrestric)
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
    /// Roles modernos - sistema novo de pap�is (opcional)
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
    /// Configura��o de interceptors e comportamentos do EF Core
    /// </summary>
    /// <param name="optionsBuilder">Builder de op��es</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Adiciona interceptor de auditoria para rastreamento autom�tico
        // de cria��o/atualiza��o de registros
        optionsBuilder.AddInterceptors(_auditInterceptor);

        base.OnConfiguring(optionsBuilder);
    }

    /// <summary>
    /// Configura��o do modelo de dados - mapeamentos e relacionamentos
    /// </summary>
    /// <param name="modelBuilder">Builder do modelo</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ==================================================================================
        // APLICA��O DAS CONFIGURA��ES DE ENTIDADES
        // ==================================================================================

        // Aplica configura��o espec�fica do User (j� existente)
        modelBuilder.ApplyConfiguration(new UserConfig());

        // Aplica todas as novas configura��es
        modelBuilder.ApplyConfiguration(new SistemaConfig());
        modelBuilder.ApplyConfiguration(new FuncaoConfig());
        modelBuilder.ApplyConfiguration(new BotaoFuncaoConfig());
        modelBuilder.ApplyConfiguration(new UserGroupConfig());
        modelBuilder.ApplyConfiguration(new GrupoDeUsuarioConfig());
        modelBuilder.ApplyConfiguration(new GrupoFuncaoConfig());

        // OU usar aplica��o autom�tica de todas as configura��es:
        // modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfig).Assembly);

        // ==================================================================================
        // CONFIGURA��ES GLOBAIS
        // ==================================================================================

        ConfigureGlobalConventions(modelBuilder);
        ConfigureLegacyBehaviors(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Configura��es globais aplicadas a todas as entidades
    /// </summary>
    /// <param name="modelBuilder">Builder do modelo</param>
    private static void ConfigureGlobalConventions(ModelBuilder modelBuilder)
    {
        // ==================================================================================
        // CONVEN��ES DE NOMEA��O
        // ==================================================================================

        // Remove pluraliza��o autom�tica de tabelas (se habilitada)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Preserva nomes de tabela configurados explicitamente
            if (entityType.GetTableName() == entityType.DisplayName())
            {
                entityType.SetTableName(entityType.DisplayName());
            }
        }

        // ==================================================================================
        // CONFIGURA��ES DE STRING
        // ==================================================================================

        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(string)))
        {
            // Define collation padr�o para campos string (se necess�rio)
            // Descomentee caso o banco use collation espec�fica
            // property.SetCollation("SQL_Latin1_General_CP1_CI_AS");

            // Configura��es padr�o para campos n�o configurados explicitamente
            if (property.GetMaxLength() == null && property.GetColumnType() == null)
            {
                property.SetMaxLength(255); // Tamanho padr�o para strings
            }
        }
    }

    /// <summary>
    /// Configura��es espec�ficas para compatibilidade com sistema legacy
    /// </summary>
    /// <param name="modelBuilder">Builder do modelo</param>
    private static void ConfigureLegacyBehaviors(ModelBuilder modelBuilder)
    {
        // ==================================================================================
        // SISTEMA LEGACY - SEM SOFT DELETE AUTOM�TICO
        // ==================================================================================

        // IMPORTANTE: O sistema legacy N�O usa soft delete padr�o (IsDeleted)
        // Usa campos espec�ficos como FlAtivo = 'S'/'N' na tabela tuse1
        // Por isso N�O aplicamos global query filters autom�ticos

        // Se fosse sistema moderno, usar�amos:
        // foreach (var entityType in modelBuilder.Model.GetEntityTypes()
        //     .Where(e => typeof(ISoftDeletable).IsAssignableFrom(e.ClrType)))
        // {
        //     modelBuilder.Entity(entityType.ClrType)
        //         .HasQueryFilter(CreateSoftDeleteFilter(entityType.ClrType));
        // }

        // ==================================================================================
        // CONFIGURA��ES DE PERFORMANCE
        // ==================================================================================

        // Configura��o padr�o de tracking para consultas
        // NoTracking por padr�o j� est� configurado no Program.cs:
        // .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)

        // ==================================================================================
        // CONFIGURA��ES DE TIMEZONE (SE NECESS�RIO)
        // ==================================================================================

        // Se o banco armazena datas em UTC, configurar convers�o autom�tica
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
// EXTENS�ES PARA FACILITAR QUERIES COMUNS
// ==================================================================================

/// <summary>
/// Extens�es �teis para o AppDbContext
/// Facilitam queries comuns no sistema legacy
/// </summary>
public static class AppDbContextExtensions
{
    #region User Extensions

    /// <summary>
    /// Obt�m usu�rios ativos (FlAtivo = 'S')
    /// </summary>
    /// <param name="users">DbSet de usu�rios</param>
    /// <returns>Query filtrada para usu�rios ativos</returns>
    public static IQueryable<User> Ativos(this DbSet<User> users)
    {
        return users.Where(u => u.FlAtivo == 'S');
    }

    /// <summary>
    /// Obt�m usu�rios por tipo
    /// </summary>
    /// <param name="users">DbSet de usu�rios</param>
    /// <param name="tipo">Tipo do usu�rio (A/U/S/C)</param>
    /// <returns>Query filtrada por tipo</returns>
    public static IQueryable<User> PorTipo(this DbSet<User> users, char tipo)
    {
        return users.Where(u => u.TpUsuario == tipo);
    }

    /// <summary>
    /// Obt�m usu�rios de uma empresa
    /// </summary>
    /// <param name="users">DbSet de usu�rios</param>
    /// <param name="cdEmpresa">C�digo da empresa</param>
    /// <returns>Query filtrada por empresa</returns>
    public static IQueryable<User> DaEmpresa(this DbSet<User> users, int cdEmpresa)
    {
        return users.Where(u => u.CdEmpresa == cdEmpresa);
    }

    #endregion

    #region Sistema Extensions

    /// <summary>
    /// Obt�m sistemas ativos
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
    /// Obt�m grupos ativos de um usu�rio (sem data fim)
    /// </summary>
    /// <param name="userGroups">DbSet de user groups</param>
    /// <param name="cdUsuario">C�digo do usu�rio</param>
    /// <returns>Query filtrada para grupos ativos</returns>
    public static IQueryable<UserGroup> AtivosDoUsuario(this DbSet<UserGroup> userGroups, string cdUsuario)
    {
        return userGroups.Where(ug =>
            ug.CdUsuario == cdUsuario &&
            ug.DtFimVal == null);
    }

    /// <summary>
    /// Obt�m grupos v�lidos em uma data espec�fica
    /// </summary>
    /// <param name="userGroups">DbSet de user groups</param>
    /// <param name="cdUsuario">C�digo do usu�rio</param>
    /// <param name="data">Data de refer�ncia</param>
    /// <returns>Query filtrada para grupos v�lidos na data</returns>
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
    /// Obt�m permiss�es de um grupo espec�fico
    /// </summary>
    /// <param name="grupoFuncoes">DbSet de grupo fun��es</param>
    /// <param name="cdGrUser">C�digo do grupo</param>
    /// <returns>Query filtrada para permiss�es do grupo</returns>
    public static IQueryable<GrupoFuncao> DoGrupo(this DbSet<GrupoFuncao> grupoFuncoes, string cdGrUser)
    {
        return grupoFuncoes.Where(gf => gf.CdGrUser == cdGrUser);
    }

    /// <summary>
    /// Obt�m permiss�es de um sistema espec�fico
    /// </summary>
    /// <param name="grupoFuncoes">DbSet de grupo fun��es</param>
    /// <param name="cdSistema">C�digo do sistema</param>
    /// <returns>Query filtrada para permiss�es do sistema</returns>
    public static IQueryable<GrupoFuncao> DoSistema(this DbSet<GrupoFuncao> grupoFuncoes, string cdSistema)
    {
        return grupoFuncoes.Where(gf => gf.CdSistema == cdSistema);
    }

    /// <summary>
    /// Verifica se grupo tem acesso a uma fun��o espec�fica
    /// </summary>
    /// <param name="grupoFuncoes">DbSet de grupo fun��es</param>
    /// <param name="cdGrUser">C�digo do grupo</param>
    /// <param name="cdSistema">C�digo do sistema</param>
    /// <param name="cdFuncao">C�digo da fun��o</param>
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
/// Constantes �teis para o sistema legacy
/// </summary>
public static class LegacyConstants
{
    #region Status de Usu�rio

    public const char USUARIO_ATIVO = 'S';
    public const char USUARIO_INATIVO = 'N';

    #endregion

    #region Tipos de Usu�rio

    public const char TIPO_ADMINISTRADOR = 'A';
    public const char TIPO_USUARIO = 'U';
    public const char TIPO_SISTEMA = 'S';
    public const char TIPO_CONSULTA = 'C';

    #endregion

    #region C�digos de A��o

    public const char ACAO_INCLUIR = 'I';
    public const char ACAO_ALTERAR = 'A';
    public const char ACAO_EXCLUIR = 'E';
    public const char ACAO_CONSULTAR = 'C';

    public const string ACOES_COMPLETAS = "IAEC";
    public const string ACOES_LEITURA = "C";
    public const string ACOES_ESCRITA = "IAE";

    #endregion

    #region C�digos de Sistema

    public const string SISTEMA_SEGURANCA = "SEG";
    public const string SISTEMA_RH = "RHU";
    public const string SISTEMA_FINANCEIRO = "FIN";
    public const string SISTEMA_ESTOQUE = "EST";

    #endregion
}