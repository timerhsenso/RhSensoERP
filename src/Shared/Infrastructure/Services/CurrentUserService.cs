using Microsoft.AspNetCore.Http;
using RhSensoERP.Shared.Core.Abstractions;
using System;
using System.Security.Claims;

namespace RhSensoERP.Shared.Infrastructure.Services;

/// <summary>
/// Implementação padrão do usuário atual baseada em Claims.
/// Compatível com Cookie Authentication e JWT.
/// ✅ ATUALIZADO: Com logs de debug para TenantId
/// </summary>
public sealed class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                return null;

            var raw =
                user.FindFirstValue(ClaimTypes.NameIdentifier) ??
                user.FindFirstValue("sub") ??
                user.FindFirstValue("userId");

            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }

    public string? UserName
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                return null;

            return
                user.FindFirstValue(ClaimTypes.Name) ??
                user.FindFirstValue("name") ??
                user.FindFirstValue("preferred_username") ??
                user.FindFirstValue("username");
        }
    }

    public Guid TenantId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
            {
                Console.WriteLine("❌ [TENANT-DEBUG] Usuário NÃO autenticado");
                return Guid.Empty;
            }

            // ✅ DEBUG: Lista TODAS as claims
            Console.WriteLine("🔍 [TENANT-DEBUG] === TODAS AS CLAIMS ===");
            foreach (var claim in user.Claims)
            {
                Console.WriteLine($"  - {claim.Type} = {claim.Value}");
            }
            Console.WriteLine("🔍 [TENANT-DEBUG] ========================");

            var raw =
                user.FindFirstValue("tenantId") ??
                user.FindFirstValue("IdSaas") ??
                user.FindFirstValue("tenant_id");

            Console.WriteLine($"🔍 [TENANT-DEBUG] TenantId RAW: '{raw ?? "NULL"}'");

            var result = Guid.TryParse(raw, out var tenantId) ? tenantId : Guid.Empty;

            Console.WriteLine($"🔍 [TENANT-DEBUG] TenantId FINAL: {result} (IsEmpty: {result == Guid.Empty})");

            return result;
        }
    }
}