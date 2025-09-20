using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RhSensoERP.API;
using RhSensoERP.Infrastructure.Persistence;
using RhSensoERP.Core.Security.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

namespace RhSensoERP.Tests.Unit.Infrastructure;

public class EntityMappingTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public EntityMappingTests(WebApplicationFactory<Program> factory) { _factory = factory; }

    [Fact]
    public async Task User_Entity_Should_Have_Correct_Structure()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await context.Users.FirstOrDefaultAsync();
        if (user != null)
        {
            user.CdUsuario.Should().NotBeNullOrEmpty("CdUsuario é obrigatório");
            user.DcUsuario.Should().NotBeNullOrEmpty("DcUsuario é obrigatório");
            user.NoUser.Should().BeGreaterThan(0, "NoUser deve ser positivo");
            user.FlAtivo.Should().BeOneOf('S', 'N');
            user.TpUsuario.Should().BeOneOf('A', 'U', 'S', 'C');
            user.Id.Should().NotBe(Guid.Empty, "Id deve ser um GUID válido");
            Console.WriteLine($"User encontrado: {user.CdUsuario} - {user.DcUsuario}");
        }
        else Console.WriteLine("Nenhum usuário encontrado na tabela tuse1");
    }

    [Fact]
    public async Task Sistema_Entity_Should_Have_Correct_Structure()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var sistema = await context.Sistemas.FirstOrDefaultAsync();
        if (sistema != null)
        {
            sistema.CdSistema.Should().NotBeNullOrEmpty("CdSistema é obrigatório");
            sistema.DcSistema.Should().NotBeNullOrEmpty("DcSistema é obrigatório");
            sistema.CdSistema.Length.Should().BeLessOrEqualTo(10, "CdSistema é char(10)");
            sistema.DcSistema.Length.Should().BeLessOrEqualTo(60, "DcSistema é varchar(60)");
            Console.WriteLine($"Sistema encontrado: {sistema.CdSistema} - {sistema.DcSistema}");
        }
        else Console.WriteLine("Nenhum sistema encontrado na tabela tsistema");
    }

    [Fact]
    public async Task Funcao_Entity_Should_Have_Correct_Structure()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var funcao = await context.Funcoes.FirstOrDefaultAsync();
        if (funcao != null)
        {
            funcao.CdFuncao.Should().NotBeNullOrEmpty("CdFuncao é obrigatório");
            funcao.CdSistema.Should().NotBeNullOrEmpty("CdSistema é obrigatório");
            funcao.CdFuncao.Length.Should().BeLessOrEqualTo(30, "CdFuncao é varchar(30)");
            funcao.CdSistema.Length.Should().BeLessOrEqualTo(10, "CdSistema é char(10)");
            Console.WriteLine($"Funcao encontrada: {funcao.CdSistema}.{funcao.CdFuncao}");
        }
        else Console.WriteLine("Nenhuma função encontrada na tabela fucn1");
    }

    [Fact]
    public async Task BotaoFuncao_Entity_Should_Have_Correct_Structure()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var botao = await context.BotoesFuncao.FirstOrDefaultAsync();
        if (botao != null)
        {
            botao.CdFuncao.Should().NotBeNullOrEmpty("CdFuncao é obrigatório");
            botao.CdSistema.Should().NotBeNullOrEmpty("CdSistema é obrigatório");
            botao.NmBotao.Should().NotBeNullOrEmpty("NmBotao é obrigatório");
            botao.DcBotao.Should().NotBeNullOrEmpty("DcBotao é obrigatório");
            botao.CdAcao.Should().BeOneOf('I', 'A', 'E', 'C');
            Console.WriteLine($"Botao encontrado: {botao.CdSistema}.{botao.CdFuncao}.{botao.NmBotao} ({botao.CdAcao})");
        }
        else Console.WriteLine("Nenhum botão encontrado na tabela btfuncao");
    }

    [Fact]
    public async Task UserGroup_Entity_Should_Have_Correct_Structure()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userGroup = await context.UserGroups.FirstOrDefaultAsync();
        if (userGroup != null)
        {
            userGroup.CdUsuario.Should().NotBeNullOrEmpty("CdUsuario é obrigatório");
            userGroup.CdGrUser.Should().NotBeNullOrEmpty("CdGrUser é obrigatório");
            userGroup.DtIniVal.Should().NotBe(default(DateTime), "DtIniVal deve ter valor");
            userGroup.Id.Should().NotBe(Guid.Empty, "Id deve ser um GUID válido");
            Console.WriteLine($"UserGroup encontrado: {userGroup.CdUsuario} -> {userGroup.CdGrUser}");
        }
        else Console.WriteLine("Nenhum user group encontrado na tabela usrh1");
    }

    [Fact]
    public async Task GrupoDeUsuario_Entity_Should_Have_Correct_Structure()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var grupo = await context.GruposDeUsuario.FirstOrDefaultAsync();
        if (grupo != null)
        {
            grupo.CdGrUser.Should().NotBeNullOrEmpty("CdGrUser é obrigatório");
            grupo.CdSistema.Should().NotBeNullOrEmpty("CdSistema é obrigatório");
            grupo.Id.Should().NotBe(Guid.Empty, "Id deve ser um GUID válido");
            Console.WriteLine($"GrupoDeUsuario encontrado: {grupo.CdSistema}.{grupo.CdGrUser}");
        }
        else Console.WriteLine("Nenhum grupo encontrado na tabela gurh1");
    }

    [Fact]
    public async Task GrupoFuncao_Entity_Should_Have_Correct_Structure()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var grupoFuncao = await context.GruposFuncoes.FirstOrDefaultAsync();
        if (grupoFuncao != null)
        {
            grupoFuncao.CdGrUser.Should().NotBeNullOrEmpty("CdGrUser é obrigatório");
            grupoFuncao.CdFuncao.Should().NotBeNullOrEmpty("CdFuncao é obrigatório");
            grupoFuncao.CdAcoes.Should().NotBeNullOrEmpty("CdAcoes é obrigatório");
            grupoFuncao.CdRestric.Should().NotBe('\0', "CdRestric deve ter valor");
            grupoFuncao.Id.Should().NotBe(Guid.Empty, "Id deve ser um GUID válido");
            Console.WriteLine($"GrupoFuncao encontrado: {grupoFuncao.CdGrUser} -> {grupoFuncao.CdFuncao} ({grupoFuncao.CdAcoes})");
        }
        else Console.WriteLine("Nenhuma permissão encontrada na tabela hbrh1");
    }
}
