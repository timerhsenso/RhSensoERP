// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Tuse1
// Module: Identity
// Data: 2026-02-25 21:55:51
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Identity.Tuse1;

/// <summary>
/// Request para atualização de Usuários.
/// Compatível com backend: UpdateTuse1Request
/// </summary>
public class UpdateTuse1Request
{
    /// <summary>
    /// Cd Usuario
    /// </summary>
    [Display(Name = "Cd Usuario")]
    [Required(ErrorMessage = "Cd Usuario é obrigatório")]
    [StringLength(30, ErrorMessage = "Cd Usuario deve ter no máximo {1} caracteres")]
    public string Cdusuario { get; set; } = string.Empty;

    /// <summary>
    /// Dc Usuario
    /// </summary>
    [Display(Name = "Dc Usuario")]
    [Required(ErrorMessage = "Dc Usuario é obrigatório")]
    [StringLength(50, ErrorMessage = "Dc Usuario deve ter no máximo {1} caracteres")]
    public string Dcusuario { get; set; } = string.Empty;

    /// <summary>
    /// Senha User
    /// </summary>
    [Display(Name = "Senha User")]
    public string Senhauser { get; set; } = string.Empty;

    /// <summary>
    /// Password Hash
    /// </summary>
    [Display(Name = "Password Hash")]
    public string Passwordhash { get; set; } = string.Empty;

    /// <summary>
    /// Password Salt
    /// </summary>
    [Display(Name = "Password Salt")]
    public string PassworDsalt { get; set; } = string.Empty;

    /// <summary>
    /// Normalized User Name
    /// </summary>
    [Display(Name = "Normalized User Name")]
    public string Normalizedusername { get; set; } = string.Empty;

    /// <summary>
    /// Nm Impcche
    /// </summary>
    [Display(Name = "Nm Impcche")]
    public string NmimPcche { get; set; } = string.Empty;

    /// <summary>
    /// Tp Usuario
    /// </summary>
    [Display(Name = "Tp Usuario")]
    [Required(ErrorMessage = "Tp Usuario é obrigatório")]
    public string Tpusuario { get; set; } = string.Empty;

    /// <summary>
    /// No Matric
    /// </summary>
    [Display(Name = "No Matric")]
    public string Nomatric { get; set; } = string.Empty;

    /// <summary>
    /// Cd Empresa
    /// </summary>
    [Display(Name = "Cd Empresa")]
    public int? Cdempresa { get; set; }

    /// <summary>
    /// Cd Filial
    /// </summary>
    [Display(Name = "Cd Filial")]
    public int? Cdfilial { get; set; }

    /// <summary>
    /// No User
    /// </summary>
    [Display(Name = "No User")]
    [Required(ErrorMessage = "No User é obrigatório")]
    public int Nouser { get; set; }

    /// <summary>
    /// Email_ Usuario
    /// </summary>
    [Display(Name = "Email_ Usuario")]
    public string EmailUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Fl Ativo
    /// </summary>
    [Display(Name = "Fl Ativo")]
    [Required(ErrorMessage = "Fl Ativo é obrigatório")]
    public string Flativo { get; set; } = string.Empty;

    /// <summary>
    /// Fl Nao Recebe Email
    /// </summary>
    [Display(Name = "Fl Nao Recebe Email")]
    public string Flnaorecebeemail { get; set; } = string.Empty;

    /// <summary>
    /// Id Funcionario
    /// </summary>
    [Display(Name = "Id Funcionario")]
    public Guid? Idfuncionario { get; set; }

    /// <summary>
    /// Tenant Principal
    /// </summary>
    [Display(Name = "Tenant Principal")]
    public Guid? TenanTprincipal { get; set; }

    /// <summary>
    /// Auth Mode
    /// </summary>
    [Display(Name = "Auth Mode")]
    [Required(ErrorMessage = "Auth Mode é obrigatório")]
    public string Authmode { get; set; } = string.Empty;

    /// <summary>
    /// Email Confirmed
    /// </summary>
    [Display(Name = "Email Confirmed")]
    [Required(ErrorMessage = "Email Confirmed é obrigatório")]
    public bool Emailconfirmed { get; set; }

    /// <summary>
    /// Email Confirmation Token
    /// </summary>
    [Display(Name = "Email Confirmation Token")]
    public string Emailconfirmationtoken { get; set; } = string.Empty;

    /// <summary>
    /// Email Confirmation Token Expiry
    /// </summary>
    [Display(Name = "Email Confirmation Token Expiry")]
    public DateTime? Emailconfirmationtokenexpiry { get; set; }

    /// <summary>
    /// Password Reset Token
    /// </summary>
    [Display(Name = "Password Reset Token")]
    public string Passwordresettoken { get; set; } = string.Empty;

    /// <summary>
    /// Password Reset Token Expiry
    /// </summary>
    [Display(Name = "Password Reset Token Expiry")]
    public DateTime? Passwordresettokenexpiry { get; set; }

    /// <summary>
    /// Password Reset Requested At
    /// </summary>
    [Display(Name = "Password Reset Requested At")]
    public DateTime? PasswordresetrequeStedat { get; set; }

    /// <summary>
    /// Password Reset Requested By
    /// </summary>
    [Display(Name = "Password Reset Requested By")]
    public string PasswordresetrequeStedby { get; set; } = string.Empty;

    /// <summary>
    /// Login Attempts
    /// </summary>
    [Display(Name = "Login Attempts")]
    [Required(ErrorMessage = "Login Attempts é obrigatório")]
    public int Loginattempts { get; set; }

    /// <summary>
    /// Locked Until
    /// </summary>
    [Display(Name = "Locked Until")]
    public DateTime? Lockeduntil { get; set; }

    /// <summary>
    /// Last Failed Login At
    /// </summary>
    [Display(Name = "Last Failed Login At")]
    public DateTime? LaStfailedloginat { get; set; }

    /// <summary>
    /// Last Login At
    /// </summary>
    [Display(Name = "Last Login At")]
    public DateTime? LaStloginat { get; set; }

    /// <summary>
    /// Last Password Changed At
    /// </summary>
    [Display(Name = "Last Password Changed At")]
    public DateTime? LaStpassworDchangedat { get; set; }

    /// <summary>
    /// Two Factor Enabled
    /// </summary>
    [Display(Name = "Two Factor Enabled")]
    [Required(ErrorMessage = "Two Factor Enabled é obrigatório")]
    public bool Twofactorenabled { get; set; }

    /// <summary>
    /// Two Factor Secret
    /// </summary>
    [Display(Name = "Two Factor Secret")]
    public string Twofactorsecret { get; set; } = string.Empty;

    /// <summary>
    /// Two Factor Backup Codes
    /// </summary>
    [Display(Name = "Two Factor Backup Codes")]
    public string TwofactorbackuPcodes { get; set; } = string.Empty;

    /// <summary>
    /// Last User Agent
    /// </summary>
    [Display(Name = "Last User Agent")]
    public string LaStuseragent { get; set; } = string.Empty;

    /// <summary>
    /// Last Ip Address
    /// </summary>
    [Display(Name = "Last Ip Address")]
    public string LaStipaddress { get; set; } = string.Empty;
}
