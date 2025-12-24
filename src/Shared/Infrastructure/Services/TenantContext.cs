// ============================================================================
// ARQUIVO NOVO - COPIAR PARA: src/Shared/Infrastructure/Services/TenantContext.cs
// ============================================================================

using Microsoft.AspNetCore.Http;
using RhSensoERP.Shared.Core.Abstractions;

namespace RhSensoERP.Shared.Infrastructure.Services;

/// <summary>
/// Implementação do contexto de tenant que resolve o tenant atual
/// baseado no HttpContext ou em configuração.
/// </summary>
public sealed class TenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Identificador lógico do tenant (SaaS).
    /// Resolvido via middleware TenantMiddleware.
    /// </summary>
    public string? TenantId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            // Tenta obter do HttpContext.Items (definido pelo TenantMiddleware)
            if (httpContext.Items.TryGetValue("TenantId", out var tenantId))
            {
                return tenantId?.ToString();
            }

            // Tenta obter do claim (se estiver autenticado)
            var tenantClaim = httpContext.User?.FindFirst("tenant_id");
            if (tenantClaim != null)
            {
                return tenantClaim.Value;
            }

            return null;
        }
    }

    /// <summary>
    /// Identificador da empresa (quando aplicável).
    /// Resolvido via claims do usuário autenticado.
    /// </summary>
    public int? EmpresaId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            // Tenta obter do HttpContext.Items
            if (httpContext.Items.TryGetValue("EmpresaId", out var empresaId))
            {
                if (empresaId != null && int.TryParse(empresaId.ToString(), out var id))
                {
                    return id;
                }
            }

            // Tenta obter do claim
            var empresaClaim = httpContext.User?.FindFirst("empresa_id");
            if (empresaClaim != null && int.TryParse(empresaClaim.Value, out var empresaIdFromClaim))
            {
                return empresaIdFromClaim;
            }

            return null;
        }
    }

    /// <summary>
    /// Identificador da filial (quando aplicável).
    /// Resolvido via claims do usuário autenticado.
    /// </summary>
    public int? FilialId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            // Tenta obter do HttpContext.Items
            if (httpContext.Items.TryGetValue("FilialId", out var filialId))
            {
                if (filialId != null && int.TryParse(filialId.ToString(), out var id))
                {
                    return id;
                }
            }

            // Tenta obter do claim
            var filialClaim = httpContext.User?.FindFirst("filial_id");
            if (filialClaim != null && int.TryParse(filialClaim.Value, out var filialIdFromClaim))
            {
                return filialIdFromClaim;
            }

            return null;
        }
    }
}
