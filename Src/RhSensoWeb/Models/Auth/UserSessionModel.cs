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
    public char FlAtivo { get; set; }
    public string? CdEmpresa { get; set; }
    public string? CdFilial { get; set; }
    public string? IdSaas { get; set; }
    public string AccessToken { get; set; } = string.Empty;
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
            new(ClaimTypes.Name, DcUsuario),
            new(ClaimTypes.Email, EmailUsuario),
            new("TpUsuario", TpUsuario),
            new("FlAtivo", FlAtivo.ToString()),
            new("AccessToken", AccessToken),
            new("LoginTime", LoginTime.ToString("O")),
            new("LastActivity", LastActivity.ToString("O"))
        };

        // Adicionar empresa e filial se existirem
        if (!string.IsNullOrEmpty(CdEmpresa))
            claims.Add(new Claim("CdEmpresa", CdEmpresa));

        if (!string.IsNullOrEmpty(CdFilial))
            claims.Add(new Claim("CdFilial", CdFilial));

        if (!string.IsNullOrEmpty(IdSaas))
            claims.Add(new Claim("IdSaas", IdSaas));

        // Adicionar grupos
        foreach (var group in Groups)
        {
            claims.Add(new Claim("Group", group.CdGrUser));
        }

        // Adicionar permissões
        foreach (var permission in Permissions)
        {
            claims.Add(new Claim("Permission", permission.PermissionKey));
            
            if (permission.CanInclude)
                claims.Add(new Claim($"Permission:{permission.PermissionKey}:I", "true"));
            
            if (permission.CanUpdate)
                claims.Add(new Claim($"Permission:{permission.PermissionKey}:A", "true"));
            
            if (permission.CanDelete)
                claims.Add(new Claim($"Permission:{permission.PermissionKey}:E", "true"));
            
            if (permission.CanConsult)
                claims.Add(new Claim($"Permission:{permission.PermissionKey}:C", "true"));
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
            FlAtivo = char.Parse(principal.FindFirst("FlAtivo")?.Value ?? "N"),
            CdEmpresa = principal.FindFirst("CdEmpresa")?.Value,
            CdFilial = principal.FindFirst("CdFilial")?.Value,
            IdSaas = principal.FindFirst("IdSaas")?.Value,
            AccessToken = principal.FindFirst("AccessToken")?.Value ?? string.Empty
        };

        // Parse das datas
        if (DateTime.TryParse(principal.FindFirst("LoginTime")?.Value, out var loginTime))
            model.LoginTime = loginTime;

        if (DateTime.TryParse(principal.FindFirst("LastActivity")?.Value, out var lastActivity))
            model.LastActivity = lastActivity;

        // Recuperar grupos
        model.Groups = principal.FindAll("Group")
            .Select(c => new UserGroupDto { CdGrUser = c.Value })
            .ToList();

        // Recuperar permissões
        var permissionClaims = principal.FindAll("Permission")
            .Where(c => !c.Type.Contains(":"))
            .Select(c => c.Value)
            .Distinct();

        foreach (var permissionKey in permissionClaims)
        {
            var permission = new UserPermissionDto
            {
                PermissionKey = permissionKey,
                CanInclude = principal.HasClaim($"Permission:{permissionKey}:I", "true"),
                CanUpdate = principal.HasClaim($"Permission:{permissionKey}:A", "true"),
                CanDelete = principal.HasClaim($"Permission:{permissionKey}:E", "true"),
                CanConsult = principal.HasClaim($"Permission:{permissionKey}:C", "true")
            };

            model.Permissions.Add(permission);
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

        return action.ToUpper() switch
        {
            "I" => permission.CanInclude,
            "A" => permission.CanUpdate,
            "E" => permission.CanDelete,
            "C" => permission.CanConsult,
            _ => false
        };
    }
}
