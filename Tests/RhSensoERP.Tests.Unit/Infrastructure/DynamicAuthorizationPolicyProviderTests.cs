using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using RhSensoERP.Infrastructure.Auth;

namespace RhSensoERP.Tests.Unit.Infrastructure;

/// <summary>
/// Garante que o provedor dinâmico cria uma policy contendo PermissionRequirement
/// com a permissão solicitada (e.g., "SEG.USUARIO.LER").
/// </summary>
public class DynamicAuthorizationPolicyProviderTests
{
    [Fact]
    public async Task GetPolicyAsync_Should_Create_Policy_With_PermissionRequirement()
    {
        // Arrange
        var options = Options.Create(new AuthorizationOptions());
        var provider = new DynamicAuthorizationPolicyProvider(options);

        // Act
        var policy = await provider.GetPolicyAsync("SEG.USUARIO.LER");

        // Assert
        policy.Should().NotBeNull("provider deve construir policies on-the-fly");

        // Evita expression tree com pattern 'is' que gerou CS8122 em algumas versões
        var found = false;
        foreach (var req in policy!.Requirements)
        {
            if (req is PermissionRequirement pr && pr.Permission == "SEG.USUARIO.LER")
            {
                found = true;
                break;
            }
        }

        found.Should().BeTrue("a policy criada precisa conter o PermissionRequirement correto");
    }
}
