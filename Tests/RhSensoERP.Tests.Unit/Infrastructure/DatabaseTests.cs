using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using RhSensoERP.Infrastructure.Persistence;
using RhSensoERP.Infrastructure.Persistence.Interceptors;
using RhSensoERP.Core.Security.Entities;
using Xunit;
using FluentAssertions;

namespace RhSensoERP.Tests.Unit.Infrastructure;

/// <summary>
/// Testes de banco de dados - Verifica se todas as tabelas estão mapeadas corretamente
/// </summary>
public class DatabaseTests : IDisposable
{
    private readonly AppDbContext _context;

    public DatabaseTests()
    {
        // IMPORTANTE: Trocar pela sua connection string
        var connectionString = "Server=localhost;Database=bd_rhu_copenor;Integrated Security=true;TrustServerCertificate=true;";

        var httpContextAccessor = new HttpContextAccessor();
        var auditInterceptor = new AuditSaveChangesInterceptor(httpContextAccessor);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;

        _context = new AppDbContext(options, auditInterceptor);
    }

    // ==================================================================================
    // TESTES DE CONECTIVIDADE
    // ==================================================================================

    [Fact]
    public async Task Deve_Conectar_Com_Banco()
    {
        // Act & Assert
        var conectou = await _context.Database.CanConnectAsync();
        conectou.Should().BeTrue("deve conseguir conectar com o banco bd_rhu_copenor");
    }

    // ==================================================================================
    // TESTES DE LEITURA DAS TABELAS
    // ==================================================================================

    [Fact]
    public async Task Deve_Ler_Tabela_tuse1()
    {
        // Act
        var usuarios = await _context.Users.Take(5).ToListAsync();

        // Assert
        usuarios.Should().NotBeNull("deve conseguir ler tabela tuse1");

        if (usuarios.Any())
        {
            var user = usuarios.First();
            user.CdUsuario.Should().NotBeNullOrEmpty("CdUsuario deve ter valor");
            user.DcUsuario.Should().NotBeNullOrEmpty("DcUsuario deve ter valor");
            user.FlAtivo.Should().BeOneOf('S', 'N'); // CORRIGIDO: removido a string
        }
    }

    [Fact]
    public async Task Deve_Ler_Tabela_tsistema()
    {
        // Act
        var sistemas = await _context.Sistemas.Take(5).ToListAsync();

        // Assert
        sistemas.Should().NotBeNull("deve conseguir ler tabela tsistema");

        if (sistemas.Any())
        {
            var sistema = sistemas.First();
            sistema.CdSistema.Should().NotBeNullOrEmpty("CdSistema deve ter valor");
            sistema.DcSistema.Should().NotBeNullOrEmpty("DcSistema deve ter valor");
        }
    }

    [Fact]
    public async Task Deve_Ler_Tabela_fucn1()
    {
        // Act
        var funcoes = await _context.Funcoes.Take(5).ToListAsync();

        // Assert
        funcoes.Should().NotBeNull("deve conseguir ler tabela fucn1");

        if (funcoes.Any())
        {
            var funcao = funcoes.First();
            funcao.CdFuncao.Should().NotBeNullOrEmpty("CdFuncao deve ter valor");
            funcao.CdSistema.Should().NotBeNullOrEmpty("CdSistema deve ter valor");
        }
    }

    [Fact]
    public async Task Deve_Ler_Tabela_btfuncao()
    {
        // Act
        var botoes = await _context.BotoesFuncao.Take(5).ToListAsync();

        // Assert
        botoes.Should().NotBeNull("deve conseguir ler tabela btfuncao");

        if (botoes.Any())
        {
            var botao = botoes.First();
            botao.NmBotao.Should().NotBeNullOrEmpty("NmBotao deve ter valor");
            botao.CdAcao.Should().BeOneOf('I', 'A', 'E', 'C'); // CORRIGIDO: removido a string
        }
    }

    [Fact]
    public async Task Deve_Ler_Tabela_usrh1()
    {
        // Act
        var userGroups = await _context.UserGroups.Take(5).ToListAsync();

        // Assert
        userGroups.Should().NotBeNull("deve conseguir ler tabela usrh1");

        if (userGroups.Any())
        {
            var ug = userGroups.First();
            ug.CdUsuario.Should().NotBeNullOrEmpty("CdUsuario deve ter valor");
            ug.CdGrUser.Should().NotBeNullOrEmpty("CdGrUser deve ter valor");
        }
    }

    [Fact]
    public async Task Deve_Ler_Tabela_gurh1()
    {
        // Act
        var grupos = await _context.GruposDeUsuario.Take(5).ToListAsync();

        // Assert
        grupos.Should().NotBeNull("deve conseguir ler tabela gurh1");

        if (grupos.Any())
        {
            var grupo = grupos.First();
            grupo.CdGrUser.Should().NotBeNullOrEmpty("CdGrUser deve ter valor");
            grupo.CdSistema.Should().NotBeNullOrEmpty("CdSistema deve ter valor");
        }
    }

    [Fact]
    public async Task Deve_Ler_Tabela_hbrh1()
    {
        // Act
        var permissoes = await _context.GruposFuncoes.Take(5).ToListAsync();

        // Assert
        permissoes.Should().NotBeNull("deve conseguir ler tabela hbrh1");

        if (permissoes.Any())
        {
            var perm = permissoes.First();
            perm.CdGrUser.Should().NotBeNullOrEmpty("CdGrUser deve ter valor");
            perm.CdFuncao.Should().NotBeNullOrEmpty("CdFuncao deve ter valor");
            perm.CdAcoes.Should().NotBeNullOrEmpty("CdAcoes deve ter valor");
        }
    }

    // ==================================================================================
    // TESTES DE RELACIONAMENTOS
    // ==================================================================================

    [Fact]
    public async Task Deve_Carregar_Sistema_Com_Funcoes()
    {
        // Act
        var sistemaComFuncoes = await _context.Sistemas
            .Include(s => s.Funcoes)
            .FirstOrDefaultAsync();

        // Assert
        if (sistemaComFuncoes != null)
        {
            sistemaComFuncoes.Funcoes.Should().NotBeNull("relacionamento Sistema -> Funcoes deve funcionar");
        }
    }

    [Fact]
    public async Task Deve_Carregar_Funcao_Com_Botoes()
    {
        // Act
        var funcaoComBotoes = await _context.Funcoes
            .Include(f => f.Botoes)
            .FirstOrDefaultAsync();

        // Assert
        if (funcaoComBotoes != null)
        {
            funcaoComBotoes.Botoes.Should().NotBeNull("relacionamento Funcao -> Botoes deve funcionar");
        }
    }

    // ==================================================================================
    // TESTES DAS EXTENSÕES
    // ==================================================================================

    [Fact]
    public async Task Deve_Filtrar_Usuarios_Ativos()
    {
        // Act
        var usuariosAtivos = await _context.Users.Ativos().ToListAsync();
        var totalUsuarios = await _context.Users.ToListAsync();

        // Assert
        usuariosAtivos.Should().OnlyContain(u => u.FlAtivo == 'S', "deve retornar apenas usuários com FlAtivo = 'S'");
        usuariosAtivos.Count.Should().BeLessOrEqualTo(totalUsuarios.Count, "usuários ativos não podem ser mais que o total");
    }

    [Fact]
    public async Task Deve_Filtrar_Sistemas_Ativos()
    {
        // Act
        var sistemasAtivos = await _context.Sistemas.Ativos().ToListAsync();
        var totalSistemas = await _context.Sistemas.ToListAsync();

        // Assert
        sistemasAtivos.Should().OnlyContain(s => s.Ativo == true, "deve retornar apenas sistemas ativos");
        sistemasAtivos.Count.Should().BeLessOrEqualTo(totalSistemas.Count, "sistemas ativos não podem ser mais que o total");
    }

    // ==================================================================================
    // TESTE COMPLETO
    // ==================================================================================

    [Fact]
    public async Task Teste_Completo_Todas_Tabelas()
    {
        // Act - Conta registros de todas as tabelas
        var resultado = new
        {
            Users = await _context.Users.CountAsync(),
            Sistemas = await _context.Sistemas.CountAsync(),
            Funcoes = await _context.Funcoes.CountAsync(),
            BotoesFuncao = await _context.BotoesFuncao.CountAsync(),
            UserGroups = await _context.UserGroups.CountAsync(),
            GruposDeUsuario = await _context.GruposDeUsuario.CountAsync(),
            GruposFuncoes = await _context.GruposFuncoes.CountAsync()
        };

        // Assert - Todas devem ser acessíveis
        resultado.Users.Should().BeGreaterOrEqualTo(0, "tabela tuse1 deve ser acessível");
        resultado.Sistemas.Should().BeGreaterOrEqualTo(0, "tabela tsistema deve ser acessível");
        resultado.Funcoes.Should().BeGreaterOrEqualTo(0, "tabela fucn1 deve ser acessível");
        resultado.BotoesFuncao.Should().BeGreaterOrEqualTo(0, "tabela btfuncao deve ser acessível");
        resultado.UserGroups.Should().BeGreaterOrEqualTo(0, "tabela usrh1 deve ser acessível");
        resultado.GruposDeUsuario.Should().BeGreaterOrEqualTo(0, "tabela gurh1 deve ser acessível");
        resultado.GruposFuncoes.Should().BeGreaterOrEqualTo(0, "tabela hbrh1 deve ser acessível");
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}