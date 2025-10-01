using System.Security.Claims;
using RhSensoWeb.Models.Auth;

namespace RhSensoWeb.Extensions;

/// <summary>
/// Extensões para ClaimsPrincipal
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Obtém o código do usuário
    /// </summary>
    public static string GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Obtém o nome do usuário
    /// </summary>
    public static string GetUserName(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Obtém o email do usuário
    /// </summary>
    public static string GetUserEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Obtém o token JWT do usuário
    /// </summary>
    public static string GetAccessToken(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("AccessToken")?.Value ?? string.Empty;
    }

    /// <summary>
    /// Obtém o código da empresa
    /// </summary>
    public static string? GetCompanyCode(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("CdEmpresa")?.Value;
    }

    /// <summary>
    /// Obtém o código da filial
    /// </summary>
    public static string? GetBranchCode(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("CdFilial")?.Value;
    }

    /// <summary>
    /// Obtém o ID do SaaS (multi-tenant)
    /// </summary>
    public static string? GetSaasId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("IdSaas")?.Value;
    }

    /// <summary>
    /// Verifica se o usuário tem uma permissão específica
    /// </summary>
    /// <param name="principal">Principal do usuário</param>
    /// <param name="permissionKey">Chave da permissão (ex: SEG.SEG_USUARIOS.C)</param>
    /// <returns>True se tem a permissão</returns>
    public static bool HasPermission(this ClaimsPrincipal principal, string permissionKey)
    {
        if (string.IsNullOrEmpty(permissionKey)) return false;

        // Verifica se tem a permissão base
        var hasBasePermission = principal.HasClaim("Permission", permissionKey);
        if (!hasBasePermission) return false;

        // Se não especificou ação, assume consulta (C)
        if (!permissionKey.Contains('.') || permissionKey.Split('.').Length < 3)
        {
            return principal.HasClaim($"Permission:{permissionKey}:C", "true");
        }

        // Extrai a ação da chave da permissão (último caractere)
        var parts = permissionKey.Split('.');
        var lastPart = parts[^1]; // Último elemento
        
        if (lastPart.Length > 0)
        {
            var action = lastPart[^1].ToString().ToUpper(); // Último caractere
            var basePermission = string.Join(".", parts[..^1]) + "." + lastPart[..^1]; // Remove último caractere
            
            return principal.HasClaim($"Permission:{basePermission}:{action}", "true");
        }

        return false;
    }

    /// <summary>
    /// Verifica se o usuário tem uma permissão específica com ação
    /// </summary>
    /// <param name="principal">Principal do usuário</param>
    /// <param name="permissionKey">Chave da permissão base (ex: SEG.SEG_USUARIOS)</param>
    /// <param name="action">Ação (I, A, E, C)</param>
    /// <returns>True se tem a permissão</returns>
    public static bool HasPermission(this ClaimsPrincipal principal, string permissionKey, string action)
    {
        if (string.IsNullOrEmpty(permissionKey) || string.IsNullOrEmpty(action)) return false;

        var fullPermissionKey = $"{permissionKey}.{action.ToUpper()}";
        return principal.HasPermission(fullPermissionKey);
    }

    /// <summary>
    /// Obtém todas as permissões do usuário
    /// </summary>
    public static List<string> GetPermissions(this ClaimsPrincipal principal)
    {
        return principal.FindAll("Permission")
            .Where(c => !c.Type.Contains(":"))
            .Select(c => c.Value)
            .ToList();
    }

    /// <summary>
    /// Obtém todos os grupos do usuário
    /// </summary>
    public static List<string> GetGroups(this ClaimsPrincipal principal)
    {
        return principal.FindAll("Group")
            .Select(c => c.Value)
            .ToList();
    }

    /// <summary>
    /// Verifica se o usuário pertence a um grupo específico
    /// </summary>
    public static bool IsInGroup(this ClaimsPrincipal principal, string groupCode)
    {
        return principal.HasClaim("Group", groupCode);
    }

    /// <summary>
    /// Obtém o modelo de sessão completo do usuário
    /// </summary>
    public static UserSessionModel GetUserSession(this ClaimsPrincipal principal)
    {
        return UserSessionModel.FromClaims(principal);
    }

    /// <summary>
    /// Verifica se o usuário está ativo
    /// </summary>
    public static bool IsActive(this ClaimsPrincipal principal)
    {
        var flAtivo = principal.FindFirst("FlAtivo")?.Value;
        return flAtivo == "S";
    }

    /// <summary>
    /// Obtém o tipo do usuário
    /// </summary>
    public static string GetUserType(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("TpUsuario")?.Value ?? string.Empty;
    }

    /// <summary>
    /// Verifica se é um usuário administrador
    /// </summary>
    public static bool IsAdmin(this ClaimsPrincipal principal)
    {
        var userType = principal.GetUserType();
        return userType.Equals("ADMIN", StringComparison.OrdinalIgnoreCase) ||
               userType.Equals("ADMINISTRADOR", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Obtém a data/hora do último login
    /// </summary>
    public static DateTime? GetLoginTime(this ClaimsPrincipal principal)
    {
        var loginTimeStr = principal.FindFirst("LoginTime")?.Value;
        if (DateTime.TryParse(loginTimeStr, out var loginTime))
            return loginTime;
        return null;
    }

    /// <summary>
    /// Obtém a data/hora da última atividade
    /// </summary>
    public static DateTime? GetLastActivity(this ClaimsPrincipal principal)
    {
        var lastActivityStr = principal.FindFirst("LastActivity")?.Value;
        if (DateTime.TryParse(lastActivityStr, out var lastActivity))
            return lastActivity;
        return null;
    }

    /// <summary>
    /// Verifica se o usuário tem qualquer uma das permissões especificadas
    /// </summary>
    public static bool HasAnyPermission(this ClaimsPrincipal principal, params string[] permissions)
    {
        if (permissions == null || permissions.Length == 0)
            return true;

        return permissions.Any(permission => principal.HasPermission(permission));
    }

    /// <summary>
    /// Verifica se o usuário tem todas as permissões especificadas
    /// </summary>
    public static bool HasAllPermissions(this ClaimsPrincipal principal, params string[] permissions)
    {
        if (permissions == null || permissions.Length == 0)
            return true;

        return permissions.All(permission => principal.HasPermission(permission));
    }
}
