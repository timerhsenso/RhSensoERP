using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RhSensoERP.API;
using RhSensoERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

namespace RhSensoERP.Tests.Unit.Infrastructure;

public class DatabaseConnectivityTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DatabaseConnectivityTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Should_Connect_To_Database()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var canConnect = await context.Database.CanConnectAsync();
        var serverName = context.Database.GetDbConnection().DataSource;
        var databaseName = context.Database.GetDbConnection().Database;
        canConnect.Should().BeTrue($"deve conectar ao servidor {serverName}, banco {databaseName}");
        serverName.Should().NotBeNullOrEmpty("servidor deve estar definido");
        databaseName.Should().NotBeNullOrEmpty("banco deve estar definido");
    }

    [Fact]
    public async Task Should_Access_All_Tables()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var tableCounts = new Dictionary<string, int>
        {
            ["tuse1 (Users)"] = await context.Users.CountAsync(),
            ["tsistema (Sistemas)"] = await context.Sistemas.CountAsync(),
            ["fucn1 (Funcoes)"] = await context.Funcoes.CountAsync(),
            ["btfuncao (BotoesFuncao)"] = await context.BotoesFuncao.CountAsync(),
            ["usrh1 (UserGroups)"] = await context.UserGroups.CountAsync(),
            ["gurh1 (GruposDeUsuario)"] = await context.GruposDeUsuario.CountAsync(),
            ["hbrh1 (GruposFuncoes)"] = await context.GruposFuncoes.CountAsync()
        };
        tableCounts.Should().AllSatisfy(kvp => kvp.Value.Should().BeGreaterOrEqualTo(0, $"tabela {kvp.Key} deve ser acessível"));
        foreach (var table in tableCounts)
            Console.WriteLine($"{table.Key}: {table.Value} registros");
    }
}
