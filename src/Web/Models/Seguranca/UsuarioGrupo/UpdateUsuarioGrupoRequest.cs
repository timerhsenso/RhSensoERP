// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: UsuarioGrupo
// Module: Seguranca
// Data: 2026-03-05 01:06:29
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Seguranca.UsuarioGrupo;

/// <summary>
/// Request para atualização de Usuário-Grupo.
/// Compatível com backend: UpdateUsuarioGrupoRequest
/// </summary>
public class UpdateUsuarioGrupoRequest
{
    /// <summary>
    /// Código de Usuario
    /// </summary>
    [Display(Name = "Código de Usuario")]
    [Required(ErrorMessage = "Código de Usuario é obrigatório")]
    [StringLength(30, ErrorMessage = "Código de Usuario deve ter no máximo {1} caracteres")]
    public string CdUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Código de Gr User
    /// </summary>
    [Display(Name = "Código de Gr User")]
    [Required(ErrorMessage = "Código de Gr User é obrigatório")]
    [StringLength(30, ErrorMessage = "Código de Gr User deve ter no máximo {1} caracteres")]
    public string CdGrUser { get; set; } = string.Empty;

    /// <summary>
    /// Código de Sistema
    /// </summary>
    [Display(Name = "Código de Sistema")]
    [StringLength(10, ErrorMessage = "Código de Sistema deve ter no máximo {1} caracteres")]
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Dt Ini Val
    /// </summary>
    [Display(Name = "Dt Ini Val")]
    [Required(ErrorMessage = "Dt Ini Val é obrigatório")]
    public DateTime DtInival { get; set; }

    /// <summary>
    /// Dt Fim Val
    /// </summary>
    [Display(Name = "Dt Fim Val")]
    public DateTime? DtFimval { get; set; }

    /// <summary>
    /// Usuario
    /// </summary>
    [Display(Name = "Usuario")]
    public Guid? IdUsuario { get; set; }

    /// <summary>
    /// Gr Upodeusuario
    /// </summary>
    [Display(Name = "Gr Upodeusuario")]
    public Guid? IdGrUpodeusuario { get; set; }
}
