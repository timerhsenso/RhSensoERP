using System.Linq;
using System.Security.Claims;

namespace RhSensoWeb.Models.Auth;

/// <summary>
/// Modelo para dados da sessão do usuário
/// </summary>
public class UserSessionModel
{
    public string CdUsuario { get; set; } = string.Empty;
    public string DcUsuario { get; set; } = string.Empty;
    public string EmailUsuario { get; set; } = string.Empty;
    public string TpUsuario { get; set; } = string.Empty;
    public char FlAtivo { get; set; } = 'N';

    // INTEIROS padronizados
    public int CdEmpresa { get; set; }
    public int CdFilial { get; set; }

    public string? IdSaas { get; set; }
    public string AccessToken { get; set; } = string.Empty;

    // Use os DTOs já existentes no seu projeto (não redefina aqui)
    public List<UserGroupDto> Groups { get; set; } = new();
    public List<UserPermissionDto> Permissions { get; set; } = new();

    public DateTime LoginTime { get; set; }
    public DateTime LastActivity { get; set; }

    /// <summary>
    /// Converte os dados do usuário para Claims
    /// </summary>
    public List<Claim> ToClaims()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, CdUsuario),
            new(ClaimTypes.Name,          DcUsuario),
            new(ClaimTypes.Email,         EmailUsuario),
            new("TpUsuario",              TpUsuario),
            new("FlAtivo",                FlAtivo.ToString()),
            new("AccessToken",            AccessToken),
            new("LoginTime",              LoginTime.ToString("O")),
            new("LastActivity",           LastActivity.ToString("O")),

            // Empresa/Filial inteiros gravados como string
            new("CdEmpresa",              CdEmpresa.ToString()),
            new("CdFilial",               CdFilial.ToString())
        };

        if (!string.IsNullOrWhiteSpace(IdSaas))
            claims.Add(new Claim("IdSaas", IdSaas));

        // Grupos
        foreach (var g in Groups)
        {
            if (!string.IsNullOrWhiteSpace(g.CdGrUser))
                claims.Add(new Claim("Group", g.CdGrUser));
        }

        // Permissões
        foreach (var p in Permissions)
        {
            if (string.IsNullOrWhiteSpace(p.PermissionKey)) continue;

            claims.Add(new Claim("Permission", p.PermissionKey));

            if (p.CanInclude) claims.Add(new Claim($"Permission:{p.PermissionKey}:I", "true"));
            if (p.CanUpdate) claims.Add(new Claim($"Permission:{p.PermissionKey}:A", "true"));
            if (p.CanDelete) claims.Add(new Claim($"Permission:{p.PermissionKey}:E", "true"));
            if (p.CanConsult) claims.Add(new Claim($"Permission:{p.PermissionKey}:C", "true"));
        }

        return claims;
    }

    /// <summary>
    /// Cria um UserSessionModel a partir de Claims
    /// </summary>
    public static UserSessionModel FromClaims(ClaimsPrincipal principal)
    {
        var model = new UserSessionModel
        {
            CdUsuario = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
            DcUsuario = principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
            EmailUsuario = principal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
            TpUsuario = principal.FindFirst("TpUsuario")?.Value ?? string.Empty,
            FlAtivo = char.TryParse(principal.FindFirst("FlAtivo")?.Value, out var fl) ? fl : 'N',
            IdSaas = principal.FindFirst("IdSaas")?.Value,
            AccessToken = principal.FindFirst("AccessToken")?.Value ?? string.Empty
        };

        // Empresa / Filial (int) — se não houver claim válida, fica 0
        if (int.TryParse(principal.FindFirst("CdEmpresa")?.Value, out var emp))
            model.CdEmpresa = emp;
        else
            model.CdEmpresa = 0;

        if (int.TryParse(principal.FindFirst("CdFilial")?.Value, out var fil))
            model.CdFilial = fil;
        else
            model.CdFilial = 0;

        // Datas
        if (DateTime.TryParse(principal.FindFirst("LoginTime")?.Value, out var loginTime))
            model.LoginTime = loginTime;

        if (DateTime.TryParse(principal.FindFirst("LastActivity")?.Value, out var lastActivity))
            model.LastActivity = lastActivity;

        // Grupos
        model.Groups = principal.FindAll("Group")
            .Select(c => new UserGroupDto { CdGrUser = c.Value })
            .ToList();

        // Permissões
        var keys = principal.FindAll("Permission")
            .Select(c => c.Value)
            .Distinct();

        foreach (var key in keys)
        {
            var perm = new UserPermissionDto
            {
                PermissionKey = key,
                CanInclude = principal.HasClaim($"Permission:{key}:I", "true"),
                CanUpdate = principal.HasClaim($"Permission:{key}:A", "true"),
                CanDelete = principal.HasClaim($"Permission:{key}:E", "true"),
                CanConsult = principal.HasClaim($"Permission:{key}:C", "true")
            };
            model.Permissions.Add(perm);
        }

        return model;
    }

    /// <summary>
    /// Verifica se o usuário tem uma permissão específica
    /// </summary>
    public bool HasPermission(string permissionKey, string action = "C")
    {
        var permission = Permissions.FirstOrDefault(p => p.PermissionKey == permissionKey);
        if (permission == null) return false;

        return action.ToUpperInvariant() switch
        {
            "I" => permission.CanInclude,
            "A" => permission.CanUpdate,
            "E" => permission.CanDelete,
            "C" => permission.CanConsult,
            _ => false
        };
    }
}
