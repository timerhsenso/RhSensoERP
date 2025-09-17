using Microsoft.AspNetCore.Authorization;

namespace RhSensoERP.Application.Auth;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base(policy: permission) { }
}
