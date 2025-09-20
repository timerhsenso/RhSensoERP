using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RhSensoERP.API;
using RhSensoERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

namespace RhSensoERP.Tests.Unit.Infrastructure;

public class EntityRelationshipTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public EntityRelationshipTests(WebApplicationFactory<Program> factory) { _factory = factory; }

    [Fact]
    public async Task Sistema_Should_Load_Related_Funcoes()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var sistemaComFuncoes = await context.Sistemas.Include(s => s.Funcoes).FirstOrDefaultAsync();
        if (sistemaComFuncoes != null)
        {
            sistemaComFuncoes.Funcoes.Should().NotBeNull("navegação Sistema -> Funcoes deve funcionar");
            Console.WriteLine($"Sistema {sistemaComFuncoes.CdSistema} tem {sistemaComFuncoes.Funcoes.Count} funções");
        }
        else Console.WriteLine("Nenhum sistema encontrado para testar relacionamento");
    }

    [Fact]
    public async Task Funcao_Should_Load_Related_Sistema()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var funcaoComSistema = await context.Funcoes.Include(f => f.Sistema).FirstOrDefaultAsync();
        if (funcaoComSistema != null)
        {
            funcaoComSistema.Sistema.Should().NotBeNull("navegação Funcao -> Sistema deve funcionar");
            funcaoComSistema.Sistema.CdSistema.Should().Be(funcaoComSistema.CdSistema, "FK deve corresponder à PK do sistema");
            Console.WriteLine($"Funcao {funcaoComSistema.CdFuncao} pertence ao sistema {funcaoComSistema.Sistema.CdSistema}");
        }
        else Console.WriteLine("Nenhuma função encontrada para testar relacionamento");
    }

    [Fact]
    public async Task Funcao_Should_Load_Related_Botoes()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var funcaoComBotoes = await context.Funcoes.Include(f => f.Botoes).FirstOrDefaultAsync();
        if (funcaoComBotoes != null)
        {
            funcaoComBotoes.Botoes.Should().NotBeNull("navegação Funcao -> Botoes deve funcionar");
            Console.WriteLine($"Funcao {funcaoComBotoes.CdFuncao} tem {funcaoComBotoes.Botoes.Count} botões");
        }
        else Console.WriteLine("Nenhuma função encontrada para testar relacionamento com botões");
    }

    [Fact]
    public async Task UserGroup_Should_Load_Related_User()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userGroupComUser = await context.UserGroups.Include(ug => ug.User).Where(ug => ug.IdUsuario != null).FirstOrDefaultAsync();
        if (userGroupComUser != null)
        {
            userGroupComUser.User.Should().NotBeNull("navegação UserGroup -> User deve funcionar");
            Console.WriteLine($"UserGroup {userGroupComUser.CdGrUser} ligado ao usuário {userGroupComUser.User.CdUsuario}");
        }
        else Console.WriteLine("Nenhum UserGroup com IdUsuario preenchido encontrado");
    }

    [Fact]
    public async Task Complex_Query_Should_Work()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var complexQuery = await (from s in context.Sistemas
                                 join f in context.Funcoes on s.CdSistema equals f.CdSistema into sf
                                 from func in sf.DefaultIfEmpty()
                                 join b in context.BotoesFuncao on new { func.CdSistema, func.CdFuncao } equals new { b.CdSistema, b.CdFuncao } into fb
                                 select new
                                 {
                                     Sistema = s.CdSistema,
                                     SistemaDesc = s.DcSistema,
                                     QuantidadeFuncoes = sf.Count(),
                                     QuantidadeBotoes = fb.Count()
                                 })
                                 .Distinct()
                                 .Take(5)
                                 .ToListAsync();
        complexQuery.Should().NotBeNull("query complexa deve funcionar");
        Console.WriteLine($"Query complexa retornou {complexQuery.Count} resultados");
        foreach (var item in complexQuery)
            Console.WriteLine($"  {item.Sistema}: {item.QuantidadeFuncoes} funções, {item.QuantidadeBotoes} botões");
    }
}
