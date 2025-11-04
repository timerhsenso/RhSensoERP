using Microsoft.AspNetCore.Authorization;

namespace RhSensoERP.Shared.Application.Auth;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base(policy: permission) { }
}
