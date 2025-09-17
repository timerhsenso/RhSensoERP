using Microsoft.AspNetCore.Http;
using RhSensoERP.Application.Common.Interfaces;

namespace RhSensoERP.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _http;
    public CurrentUserService(IHttpContextAccessor http) => _http = http;

    public string? UserId => _http.HttpContext?.User?.Identity?.Name;

    public Guid? TenantId
        => Guid.TryParse(_http.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "tenant")?.Value, out var id) ? id : null;
}
