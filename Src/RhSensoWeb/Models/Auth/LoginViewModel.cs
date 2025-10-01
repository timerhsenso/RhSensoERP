using System.ComponentModel.DataAnnotations;

namespace RhSensoWeb.Models.Auth;

/// <summary>
/// ViewModel para tela de login
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "O usuário é obrigatório")]
    [Display(Name = "Usuário")]
    public string CdUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Senha { get; set; } = string.Empty;

    [Display(Name = "Lembrar-me")]
    public bool RememberMe { get; set; }

    [Display(Name = "Domínio")]
    public string? Dominio { get; set; }

    /// <summary>
    /// URL para redirecionamento após login
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// Mensagem de erro personalizada
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// DTO para requisição de login na API
/// </summary>
public class LoginRequestDto
{
    public string CdUsuario { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string? Dominio { get; set; }
}

/// <summary>
/// DTO para resposta de login da API
/// </summary>
public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public UserDataDto UserData { get; set; } = new();
    public List<UserGroupDto> Groups { get; set; } = new();
    public List<UserPermissionDto> Permissions { get; set; } = new();
}

/// <summary>
/// Dados do usuário logado
/// </summary>
public class UserDataDto
{
    public string CdUsuario { get; set; } = string.Empty;
    public string DcUsuario { get; set; } = string.Empty;
    public string EmailUsuario { get; set; } = string.Empty;
    public string TpUsuario { get; set; } = string.Empty;
    public char FlAtivo { get; set; }
    public string? CdEmpresa { get; set; }
    public string? CdFilial { get; set; }
    public string? IdSaas { get; set; }
}

/// <summary>
/// Grupo do usuário
/// </summary>
public class UserGroupDto
{
    public string CdGrUser { get; set; } = string.Empty;
    public string DcGrUser { get; set; } = string.Empty;
    public char FlAtivo { get; set; }
}

/// <summary>
/// Permissão do usuário
/// </summary>
public class UserPermissionDto
{
    public string CdSistema { get; set; } = string.Empty;
    public string CdFuncao { get; set; } = string.Empty;
    public string CdBotao { get; set; } = string.Empty;
    public string PermissionKey { get; set; } = string.Empty;
    public bool CanInclude { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanConsult { get; set; }
}
