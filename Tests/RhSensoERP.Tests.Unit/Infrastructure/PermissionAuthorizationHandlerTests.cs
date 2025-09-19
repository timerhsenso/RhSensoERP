using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using RhSensoERP.Infrastructure.Auth;
using Xunit;

namespace RhSensoERP.Tests.Unit.Infrastructure;

/// <summary>
/// Testes da classe <c>PermissionAuthorizationHandlerTests</c>.
/// Este arquivo documenta o objetivo de cada teste e o resultado esperado, sem alterar a lógica.
/// </summary>
/// <remarks>
/// Local: Tests/RhSensoERP.Tests.Unit/Infrastructure/PermissionAuthorizationHandlerTests.cs
/// Diretrizes: nome claro de teste; Arrange-Act-Assert explícito; asserts específicos.
/// </remarks>
public class PermissionAuthorizationHandlerTests
{
    [Fact]
/// <summary>
/// Deve succeed quando usuário has required permission.
/// </summary>
    public async Task Should_Succeed_When_User_Has_Required_Permission()
    {
        var requirement = new PermissionRequirement("SEG.USUARIO.LER");
        var handler = new PermissionAuthorizationHandler();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("perm", "SEG.USUARIO.LER") }, "test"));

        var ctx = new AuthorizationHandlerContext(new[] { requirement }, user, null);
        await handler.HandleAsync(ctx);

        ctx.HasSucceeded.Should().BeTrue();
    }

    [Fact]
/// <summary>
/// Deve not succeed quando usuário lacks permission.
/// </summary>
    public async Task Should_Not_Succeed_When_User_Lacks_Permission()
    {
        var requirement = new PermissionRequirement("SEG.USUARIO.EDITAR");
        var handler = new PermissionAuthorizationHandler();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("perm", "SEG.OUTRA.COISA") }, "test"));

        var ctx = new AuthorizationHandlerContext(new[] { requirement }, user, null);
        await handler.HandleAsync(ctx);

        ctx.HasSucceeded.Should().BeFalse();
    }
}