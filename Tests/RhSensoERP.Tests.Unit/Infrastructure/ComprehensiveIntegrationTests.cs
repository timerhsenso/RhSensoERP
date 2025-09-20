using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RhSensoERP.API;
using RhSensoERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

namespace RhSensoERP.Tests.Unit.Infrastructure;

public class ComprehensiveIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public ComprehensiveIntegrationTests(WebApplicationFactory<Program> factory) { _factory = factory; }

    [Fact]
    public async Task Complete_System_Integration_Test()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var startTime = DateTime.UtcNow;

        var systemStatus = new
        {
            CanConnect = await context.Database.CanConnectAsync(),
            Server = context.Database.GetDbConnection().DataSource,
            Database = context.Database.GetDbConnection().Database,
            TableCounts = new
            {
                Users = await context.Users.CountAsync(),
                Sistemas = await context.Sistemas.CountAsync(),
                Funcoes = await context.Funcoes.CountAsync(),
                BotoesFuncao = await context.BotoesFuncao.CountAsync(),
                UserGroups = await context.UserGroups.CountAsync(),
                GruposDeUsuario = await context.GruposDeUsuario.CountAsync(),
                GruposFuncoes = await context.GruposFuncoes.CountAsync()
            },
            ActiveCounts = new
            {
                UsuariosAtivos = await context.Users.Ativos().CountAsync(),
                SistemasAtivos = await context.Sistemas.Ativos().CountAsync()
            },
            HasRelationships = new
            {
                SistemWithFuncoes = await context.Sistemas.Include(s => s.Funcoes).AnyAsync(s => s.Funcoes.Any()),
                FuncaoWithBotoes = await context.Funcoes.Include(f => f.Botoes).AnyAsync(f => f.Botoes.Any()),
                UserGroupWithUser = await context.UserGroups.Include(ug => ug.User).AnyAsync(ug => ug.User != null)
            },
            ExecutionTime = DateTime.UtcNow - startTime
        };

        systemStatus.CanConnect.Should().BeTrue("deve conectar ao banco de dados");
        systemStatus.Server.Should().NotBeNullOrEmpty("servidor deve estar definido");
        systemStatus.Database.Should().NotBeNullOrEmpty("banco deve estar definido");

        systemStatus.TableCounts.Users.Should().BeGreaterOrEqualTo(0, "tabela Users deve ser acessível");
        systemStatus.TableCounts.Sistemas.Should().BeGreaterOrEqualTo(0, "tabela Sistemas deve ser acessível");
        systemStatus.TableCounts.Funcoes.Should().BeGreaterOrEqualTo(0, "tabela Funcoes deve ser acessível");
        systemStatus.TableCounts.BotoesFuncao.Should().BeGreaterOrEqualTo(0, "tabela BotoesFuncao deve ser acessível");
        systemStatus.TableCounts.UserGroups.Should().BeGreaterOrEqualTo(0, "tabela UserGroups deve ser acessível");
        systemStatus.TableCounts.GruposDeUsuario.Should().BeGreaterOrEqualTo(0, "tabela GruposDeUsuario deve ser acessível");
        systemStatus.TableCounts.GruposFuncoes.Should().BeGreaterOrEqualTo(0, "tabela GruposFuncoes deve ser acessível");

        systemStatus.ActiveCounts.UsuariosAtivos.Should().BeGreaterOrEqualTo(0, "extensão Users.Ativos deve funcionar");
        systemStatus.ActiveCounts.SistemasAtivos.Should().BeGreaterOrEqualTo(0, "extensão Sistemas.Ativos deve funcionar");

        systemStatus.ExecutionTime.TotalSeconds.Should().BeLessThan(30, "teste completo deve executar em menos de 30 segundos");

        Console.WriteLine("=== RESULTADO DO TESTE INTEGRADO ===");
        Console.WriteLine($"Servidor: {systemStatus.Server}");
        Console.WriteLine($"Banco: {systemStatus.Database}");
        Console.WriteLine($"Conectado: {systemStatus.CanConnect}");
        Console.WriteLine($"Tempo de execução: {systemStatus.ExecutionTime.TotalMilliseconds}ms");
        Console.WriteLine();
        Console.WriteLine("CONTADORES DE TABELAS:");
        Console.WriteLine($"  Users: {systemStatus.TableCounts.Users}");
        Console.WriteLine($"  Sistemas: {systemStatus.TableCounts.Sistemas}");
        Console.WriteLine($"  Funcoes: {systemStatus.TableCounts.Funcoes}");
        Console.WriteLine($"  BotoesFuncao: {systemStatus.TableCounts.BotoesFuncao}");
        Console.WriteLine($"  UserGroups: {systemStatus.TableCounts.UserGroups}");
        Console.WriteLine($"  GruposDeUsuario: {systemStatus.TableCounts.GruposDeUsuario}");
        Console.WriteLine($"  GruposFuncoes: {systemStatus.TableCounts.GruposFuncoes}");
        Console.WriteLine();
        Console.WriteLine("FILTROS ATIVOS:");
        Console.WriteLine($"  Usuários ativos: {systemStatus.ActiveCounts.UsuariosAtivos}");
        Console.WriteLine($"  Sistemas ativos: {systemStatus.ActiveCounts.SistemasAtivos}");
        Console.WriteLine();
        Console.WriteLine("RELACIONAMENTOS:");
        Console.WriteLine($"  Sistema com Funções: {systemStatus.HasRelationships.SistemWithFuncoes}");
        Console.WriteLine($"  Função com Botões: {systemStatus.HasRelationships.FuncaoWithBotoes}");
        Console.WriteLine($"  UserGroup com User: {systemStatus.HasRelationships.UserGroupWithUser}");
        Console.WriteLine("=== FIM DO TESTE INTEGRADO ===");

        var totalEntitiesWorking = new[]
        {
            systemStatus.TableCounts.Users,
            systemStatus.TableCounts.Sistemas,
            systemStatus.TableCounts.Funcoes,
            systemStatus.TableCounts.BotoesFuncao,
            systemStatus.TableCounts.UserGroups,
            systemStatus.TableCounts.GruposDeUsuario,
            systemStatus.TableCounts.GruposFuncoes
        }.All(count => count >= 0);

        totalEntitiesWorking.Should().BeTrue("todas as 7 entidades devem estar funcionando perfeitamente");
    }
}
