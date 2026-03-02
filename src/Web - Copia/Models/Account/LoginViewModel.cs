using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Account;

/// <summary>
/// ViewModel para login de usuário.
/// </summary>
public sealed class LoginViewModel
{
    /// <summary>
    /// Código do usuário.
    /// </summary>
    [Required(ErrorMessage = "O código do usuário é obrigatório.")]
    [Display(Name = "Usuário")]
    public string CdUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    [Required(ErrorMessage = "A senha é obrigatória.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Senha { get; set; } = string.Empty;

    /// <summary>
    /// Manter conectado.
    /// </summary>
    [Display(Name = "Manter conectado")]
    public bool RememberMe { get; set; }

    /// <summary>
    /// URL de retorno após login.
    /// </summary>
    public string? ReturnUrl { get; set; }
}