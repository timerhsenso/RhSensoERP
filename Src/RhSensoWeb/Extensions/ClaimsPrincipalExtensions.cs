using System.Security.Claims;
using System.Text.Json;
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
        return principal.FindFirst("access_token")?.Value ?? string.Empty;
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
    /// Verifica se o usuário está ativo
    /// </summary>
    public static bool IsActive(this ClaimsPrincipal principal)
    {
        var flAtivo = principal.FindFirst("FlAtivo")?.Value;
        return flAtivo == "S";
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

    // ===========================================
    // MÉTODOS COM IHttpContextAccessor (para Controllers)
    // ===========================================

    /// <summary>
    /// Obtém o modelo de sessão completo do usuário (da Session HTTP)
    /// USAR EM CONTROLLERS - requer IHttpContextAccessor
    /// </summary>
    public static UserSessionModel GetUserSession(this ClaimsPrincipal principal, IHttpContextAccessor httpContextAccessor)
    {
        var sessionJson = httpContextAccessor.HttpContext?.Session.GetString("UserSession");

        if (!string.IsNullOrEmpty(sessionJson))
        {
            try
            {
                return JsonSerializer.Deserialize<UserSessionModel>(sessionJson) ?? CreateBasicSession(principal);
            }
            catch
            {
                return CreateBasicSession(principal);
            }
        }

        return CreateBasicSession(principal);
    }

    /// <summary>
    /// Verifica se o usuário tem uma permissão específica (consulta Session)
    /// USAR EM CONTROLLERS - requer IHttpContextAccessor
    /// </summary>
    public static bool HasPermission(this ClaimsPrincipal principal, IHttpContextAccessor httpContextAccessor, string permissionKey, string action = "C")
    {
        if (string.IsNullOrEmpty(permissionKey)) return false;

        var session = principal.GetUserSession(httpContextAccessor);
        return session.HasPermission(permissionKey, action);
    }

    /// <summary>
    /// Obtém todas as permissões do usuário (da Session)
    /// USAR EM CONTROLLERS - requer IHttpContextAccessor
    /// </summary>
    public static List<UserPermissionDto> GetPermissions(this ClaimsPrincipal principal, IHttpContextAccessor httpContextAccessor)
    {
        var session = principal.GetUserSession(httpContextAccessor);
        return session.Permissions;
    }

    /// <summary>
    /// Obtém todos os grupos do usuário (da Session)
    /// USAR EM CONTROLLERS - requer IHttpContextAccessor
    /// </summary>
    public static List<UserGroupDto> GetGroups(this ClaimsPrincipal principal, IHttpContextAccessor httpContextAccessor)
    {
        var session = principal.GetUserSession(httpContextAccessor);
        return session.Groups;
    }

    /// <summary>
    /// Verifica se o usuário pertence a um grupo específico (da Session)
    /// USAR EM CONTROLLERS - requer IHttpContextAccessor
    /// </summary>
    public static bool IsInGroup(this ClaimsPrincipal principal, IHttpContextAccessor httpContextAccessor, string groupCode)
    {
        var session = principal.GetUserSession(httpContextAccessor);
        return session.Groups.Any(g => g.CdGrUser.Equals(groupCode, StringComparison.OrdinalIgnoreCase));
    }

    // ===========================================
    // MÉTODOS SEM IHttpContextAccessor (para Views/TagHelpers)
    // ===========================================

    /// <summary>
    /// Obtém sessão básica apenas com claims (sem permissões/grupos completos)
    /// USAR EM VIEWS - não requer IHttpContextAccessor
    /// </summary>
    public static UserSessionModel GetUserSession(this ClaimsPrincipal principal)
    {
        return CreateBasicSession(principal);
    }

    /// <summary>
    /// Verifica se o usuário tem uma permissão específica (formato: "SEG.SEG_USUARIOS.C")
    /// USAR EM VIEWS/TAGHELPERS - não requer IHttpContextAccessor
    /// LIMITAÇÃO: Retorna sempre TRUE porque permissões estão na Session, não nas Claims
    /// Use TagHelpers com IHttpContextAccessor para verificação real
    /// </summary>
    public static bool HasPermission(this ClaimsPrincipal principal, string permissionKey)
    {
        // ⚠️ ATENÇÃO: Permissões estão na Session, não nas Claims
        // Este método sempre retorna TRUE para compatibilidade com Views antigas
        // Use @inject IHttpContextAccessor e User.HasPermission(accessor, "chave") para verificação real
        return true;
    }

    /// <summary>
    /// Verifica se o usuário tem qualquer uma das permissões especificadas
    /// USAR EM VIEWS/TAGHELPERS - não requer IHttpContextAccessor
    /// </summary>
    public static bool HasAnyPermission(this ClaimsPrincipal principal, params string[] permissions)
    {
        // ⚠️ Mesma limitação do HasPermission
        return true;
    }

    /// <summary>
    /// Verifica se o usuário tem todas as permissões especificadas
    /// USAR EM VIEWS/TAGHELPERS - não requer IHttpContextAccessor
    /// </summary>
    public static bool HasAllPermissions(this ClaimsPrincipal principal, params string[] permissions)
    {
        // ⚠️ Mesma limitação do HasPermission
        return true;
    }

    // ===========================================
    // MÉTODOS PRIVADOS
    // ===========================================

    /// <summary>
    /// Cria sessão básica apenas com claims (sem permissões/grupos)
    /// </summary>
    private static UserSessionModel CreateBasicSession(ClaimsPrincipal principal)
    {
        return new UserSessionModel
        {
            CdUsuario = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
            DcUsuario = principal.FindFirst("DcUsuario")?.Value ?? string.Empty,
            EmailUsuario = principal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
            TpUsuario = principal.FindFirst("TpUsuario")?.Value ?? string.Empty,
            FlAtivo = char.TryParse(principal.FindFirst("FlAtivo")?.Value, out var fl) ? fl : 'N',
            AccessToken = principal.FindFirst("access_token")?.Value ?? string.Empty,
            CdEmpresa = int.TryParse(principal.FindFirst("CdEmpresa")?.Value, out var emp) ? emp : 0,
            CdFilial = int.TryParse(principal.FindFirst("CdFilial")?.Value, out var fil) ? fil : 0,
            IdSaas = principal.FindFirst("IdSaas")?.Value,
            LoginTime = principal.GetLoginTime() ?? DateTime.MinValue,
            LastActivity = principal.GetLastActivity() ?? DateTime.UtcNow,
            Groups = new List<UserGroupDto>(),
            Permissions = new List<UserPermissionDto>()
        };
    }
}