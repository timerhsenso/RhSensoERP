using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using RhSensoERP.Infrastructure.Auth;

namespace RhSensoERP.Tests.Unit.Infrastructure;

public class DynamicAuthorizationPolicyProviderTests
{
    [Fact]
    public async Task GetPolicyAsync_Should_Create_Policy_With_PermissionRequirement()
    {
        var options = Options.Create(new AuthorizationOptions());
        var provider = new DynamicAuthorizationPolicyProvider(options);

        var policy = await provider.GetPolicyAsync("SEG.USUARIO.LER");
        policy.Should().NotBeNull();

        var found = false;
        foreach (var req in policy!.Requirements)
        {
            if (req is PermissionRequirement pr && pr.Permission == "SEG.USUARIO.LER")
            {
                found = true;
                break;
            }
        }

        found.Should().BeTrue();
    }
}
