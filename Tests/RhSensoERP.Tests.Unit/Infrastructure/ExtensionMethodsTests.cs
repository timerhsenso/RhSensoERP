using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RhSensoERP.API;
using RhSensoERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

namespace RhSensoERP.Tests.Unit.Infrastructure;

public class ExtensionMethodsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public ExtensionMethodsTests(WebApplicationFactory<Program> factory) { _factory = factory; }

    [Fact]
    public async Task Users_Ativos_Extension_Should_Filter_Correctly()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var todosUsuarios = await context.Users.ToListAsync();
        var usuariosAtivos = await context.Users.Ativos().ToListAsync();
        usuariosAtivos.Should().OnlyContain(u => u.FlAtivo == 'S', "extensão Ativos deve retornar apenas usuários com FlAtivo = 'S'");
        usuariosAtivos.Count.Should().BeLessOrEqualTo(todosUsuarios.Count, "usuários ativos não podem ser mais que o total");
        Console.WriteLine($"Total usuários: {todosUsuarios.Count}, Ativos: {usuariosAtivos.Count}");
    }

    [Fact]
    public async Task Sistemas_Ativos_Extension_Should_Filter_Correctly()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var todosSistemas = await context.Sistemas.ToListAsync();
        var sistemasAtivos = await context.Sistemas.Ativos().ToListAsync();
        sistemasAtivos.Should().OnlyContain(s => s.Ativo == true, "extensão Ativos deve retornar apenas sistemas ativos");
        sistemasAtivos.Count.Should().BeLessOrEqualTo(todosSistemas.Count, "sistemas ativos não podem ser mais que o total");
        Console.WriteLine($"Total sistemas: {todosSistemas.Count}, Ativos: {sistemasAtivos.Count}");
    }

    [Fact]
    public async Task UserGroups_AtivosDoUsuario_Extension_Should_Filter_Correctly()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var primeiroUsuario = await context.UserGroups.Select(ug => ug.CdUsuario).FirstOrDefaultAsync();
        if (primeiroUsuario != null)
        {
            var todosGruposDoUsuario = await context.UserGroups.Where(ug => ug.CdUsuario == primeiroUsuario).ToListAsync();
            var gruposAtivosDoUsuario = await context.UserGroups.AtivosDoUsuario(primeiroUsuario).ToListAsync();
            gruposAtivosDoUsuario.Should().OnlyContain(ug => ug.DtFimVal == null, "extensão AtivosDoUsuario deve retornar apenas grupos sem data fim");
            gruposAtivosDoUsuario.Should().OnlyContain(ug => ug.CdUsuario == primeiroUsuario, "deve filtrar pelo usuário correto");
            gruposAtivosDoUsuario.Count.Should().BeLessOrEqualTo(todosGruposDoUsuario.Count, "grupos ativos não podem ser mais que o total do usuário");
            Console.WriteLine($"Usuário {primeiroUsuario}: {todosGruposDoUsuario.Count} grupos total, {gruposAtivosDoUsuario.Count} ativos");
        }
        else Console.WriteLine("Nenhum UserGroup encontrado para testar extensão");
    }

    [Fact]
    public async Task Performance_Extensions_Should_Be_Efficient()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var usuariosAtivos = await context.Users.Ativos().CountAsync();
        var sistemasAtivos = await context.Sistemas.Ativos().CountAsync();
        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000, "extensões devem ser rápidas (menos de 5 segundos)");
        Console.WriteLine($"Extensões executadas em {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"Usuários ativos: {usuariosAtivos}, Sistemas ativos: {sistemasAtivos}");
    }
}
