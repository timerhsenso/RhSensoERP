// =============================================================================
// RHSENSOERP WEB - CLAIMS PRINCIPAL EXTENSIONS
// =============================================================================
using System.Security.Claims;

namespace RhSensoERP.Web.Extensions;

/// <summary>
/// Métodos de extensão para facilitar o acesso a claims customizadas.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Obtém o código do usuário (cdusuario) a partir das claims.
    /// </summary>
    public static string? GetCdUsuario(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("cdusuario");
    }
}
