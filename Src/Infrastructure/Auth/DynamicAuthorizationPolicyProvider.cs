using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace RhSensoERP.Infrastructure.Auth;

public class DynamicAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public DynamicAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options) { }

    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Create a policy on-the-fly that requires a PermissionRequirement with the policyName
        var policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();
        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}
