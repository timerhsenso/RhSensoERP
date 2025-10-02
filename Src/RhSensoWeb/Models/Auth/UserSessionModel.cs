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
            new Claim(ClaimTypes.NameIdentifier, CdUsuario),
            new Claim(ClaimTypes.Name, CdUsuario),
            new Claim(ClaimTypes.Email, EmailUsuario ?? ""),
            new Claim("DcUsuario", DcUsuario ?? ""),
            new Claim("TpUsuario", TpUsuario ?? ""),
            new Claim("FlAtivo", FlAtivo.ToString()), // ✅ CORRIGIDO: char.ToString()
            new Claim("CdEmpresa", CdEmpresa.ToString()),
            new Claim("CdFilial", CdFilial.ToString()),
            new Claim("access_token", AccessToken), // ✅ MINÚSCULO
            new Claim("LoginTime", LoginTime.ToString("O")),
            new Claim("LastActivity", LastActivity.ToString("O"))
        };

        // Adicionar IdSaas se existir
        if (!string.IsNullOrEmpty(IdSaas))
        {
            claims.Add(new Claim("IdSaas", IdSaas));
        }

        // Adicionar grupos
        if (Groups != null)
        {
            foreach (var group in Groups)
            {
                claims.Add(new Claim("Group", group.CdGrUser));
            }
        }

        // Adicionar permissões
        if (Permissions != null)
        {
            foreach (var permission in Permissions)
            {
                var permKey = $"{permission.CdSistema}.{permission.CdFuncao}";
                claims.Add(new Claim("Permission", permKey));

                // ✅ CORRIGIDO: Usar propriedades booleanas ao invés de CdAcoes
                if (permission.CanInclude)
                    claims.Add(new Claim($"Permission:{permKey}:I", "true"));

                if (permission.CanUpdate)
                    claims.Add(new Claim($"Permission:{permKey}:A", "true"));

                if (permission.CanDelete)
                    claims.Add(new Claim($"Permission:{permKey}:E", "true"));

                if (permission.CanConsult)
                    claims.Add(new Claim($"Permission:{permKey}:C", "true"));
            }
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
            DcUsuario = principal.FindFirst("DcUsuario")?.Value ?? string.Empty,
            EmailUsuario = principal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
            TpUsuario = principal.FindFirst("TpUsuario")?.Value ?? string.Empty,
            FlAtivo = char.TryParse(principal.FindFirst("FlAtivo")?.Value, out var fl) ? fl : 'N',
            IdSaas = principal.FindFirst("IdSaas")?.Value,
            AccessToken = principal.FindFirst("access_token")?.Value ?? string.Empty // ✅ MINÚSCULO
        };

        // Empresa / Filial (int)
        if (int.TryParse(principal.FindFirst("CdEmpresa")?.Value, out var emp))
            model.CdEmpresa = emp;

        if (int.TryParse(principal.FindFirst("CdFilial")?.Value, out var fil))
            model.CdFilial = fil;

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
            var parts = key.Split('.');
            var perm = new UserPermissionDto
            {
                CdSistema = parts.Length > 0 ? parts[0] : "",
                CdFuncao = parts.Length > 1 ? parts[1] : "",
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