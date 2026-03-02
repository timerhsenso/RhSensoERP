// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Tuse1
// Module: Identity
// Data: 2026-02-25 21:55:51
// =============================================================================

namespace RhSensoERP.Web.Models.Identity.Tuse1;

/// <summary>
/// DTO de leitura para Usuários.
/// Compatível com backend: RhSensoERP.Modules.Identity.Application.DTOs.Tuse1Dto
/// </summary>
public class Tuse1Dto
{
    /// <summary>
    /// Cd Usuario
    /// </summary>
    public string Cdusuario { get; set; } = string.Empty;

    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Dc Usuario
    /// </summary>
    public string Dcusuario { get; set; } = string.Empty;

    /// <summary>
    /// Senha User
    /// </summary>
    public string Senhauser { get; set; } = string.Empty;

    /// <summary>
    /// Password Hash
    /// </summary>
    public string Passwordhash { get; set; } = string.Empty;

    /// <summary>
    /// Password Salt
    /// </summary>
    public string PassworDsalt { get; set; } = string.Empty;

    /// <summary>
    /// Normalized User Name
    /// </summary>
    public string Normalizedusername { get; set; } = string.Empty;

    /// <summary>
    /// Nm Impcche
    /// </summary>
    public string NmimPcche { get; set; } = string.Empty;

    /// <summary>
    /// Tp Usuario
    /// </summary>
    public string Tpusuario { get; set; } = string.Empty;

    /// <summary>
    /// No Matric
    /// </summary>
    public string Nomatric { get; set; } = string.Empty;

    /// <summary>
    /// Cd Empresa
    /// </summary>
    public int? Cdempresa { get; set; }

    /// <summary>
    /// Cd Filial
    /// </summary>
    public int? Cdfilial { get; set; }

    /// <summary>
    /// No User
    /// </summary>
    public int Nouser { get; set; }

    /// <summary>
    /// Email_ Usuario
    /// </summary>
    public string EmailUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Fl Ativo
    /// </summary>
    public string Flativo { get; set; } = string.Empty;

    /// <summary>
    /// Fl Nao Recebe Email
    /// </summary>
    public string Flnaorecebeemail { get; set; } = string.Empty;

    /// <summary>
    /// Id Funcionario
    /// </summary>
    public Guid? Idfuncionario { get; set; }

    /// <summary>
    /// Tenant Principal
    /// </summary>
    public Guid? TenanTprincipal { get; set; }

    /// <summary>
    /// Tenant Id
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Auth Mode
    /// </summary>
    public string Authmode { get; set; } = string.Empty;

    /// <summary>
    /// Email Confirmed
    /// </summary>
    public bool Emailconfirmed { get; set; }

    /// <summary>
    /// Email Confirmation Token
    /// </summary>
    public string Emailconfirmationtoken { get; set; } = string.Empty;

    /// <summary>
    /// Email Confirmation Token Expiry
    /// </summary>
    public DateTime? Emailconfirmationtokenexpiry { get; set; }

    /// <summary>
    /// Password Reset Token
    /// </summary>
    public string Passwordresettoken { get; set; } = string.Empty;

    /// <summary>
    /// Password Reset Token Expiry
    /// </summary>
    public DateTime? Passwordresettokenexpiry { get; set; }

    /// <summary>
    /// Password Reset Requested At
    /// </summary>
    public DateTime? PasswordresetrequeStedat { get; set; }

    /// <summary>
    /// Password Reset Requested By
    /// </summary>
    public string PasswordresetrequeStedby { get; set; } = string.Empty;

    /// <summary>
    /// Login Attempts
    /// </summary>
    public int Loginattempts { get; set; }

    /// <summary>
    /// Locked Until
    /// </summary>
    public DateTime? Lockeduntil { get; set; }

    /// <summary>
    /// Last Failed Login At
    /// </summary>
    public DateTime? LaStfailedloginat { get; set; }

    /// <summary>
    /// Last Login At
    /// </summary>
    public DateTime? LaStloginat { get; set; }

    /// <summary>
    /// Last Password Changed At
    /// </summary>
    public DateTime? LaStpassworDchangedat { get; set; }

    /// <summary>
    /// Two Factor Enabled
    /// </summary>
    public bool Twofactorenabled { get; set; }

    /// <summary>
    /// Two Factor Secret
    /// </summary>
    public string Twofactorsecret { get; set; } = string.Empty;

    /// <summary>
    /// Two Factor Backup Codes
    /// </summary>
    public string TwofactorbackuPcodes { get; set; } = string.Empty;

    /// <summary>
    /// Createdat
    /// </summary>
    public DateTime Createdat { get; set; }

    /// <summary>
    /// Updatedat
    /// </summary>
    public DateTime Updatedat { get; set; }

    /// <summary>
    /// Createdby
    /// </summary>
    public string Createdby { get; set; } = string.Empty;

    /// <summary>
    /// Updatedby
    /// </summary>
    public string Updatedby { get; set; } = string.Empty;

    /// <summary>
    /// Last User Agent
    /// </summary>
    public string LaStuseragent { get; set; } = string.Empty;

    /// <summary>
    /// Last Ip Address
    /// </summary>
    public string LaStipaddress { get; set; } = string.Empty;
}
