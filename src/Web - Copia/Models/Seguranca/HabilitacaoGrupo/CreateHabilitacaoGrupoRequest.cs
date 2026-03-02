// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: HabilitacaoGrupo
// Module: Seguranca
// Data: 2026-03-02 17:57:18
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Seguranca.HabilitacaoGrupo;

/// <summary>
/// Request para criação de Habilitação de Grupo.
/// Compatível com backend: CreateHabilitacaoGrupoRequest
/// </summary>
public class CreateHabilitacaoGrupoRequest
{
    /// <summary>
    /// Código de Gr User
    /// </summary>
    [Display(Name = "Código de Gr User")]
    [Required(ErrorMessage = "Código de Gr User é obrigatório")]
    [StringLength(30, ErrorMessage = "Código de Gr User deve ter no máximo {1} caracteres")]
    public string CdGrUser { get; set; } = string.Empty;

    /// <summary>
    /// Código de Funcao
    /// </summary>
    [Display(Name = "Código de Funcao")]
    [Required(ErrorMessage = "Código de Funcao é obrigatório")]
    [StringLength(30, ErrorMessage = "Código de Funcao deve ter no máximo {1} caracteres")]
    public string CdFuncao { get; set; } = string.Empty;

    /// <summary>
    /// Código de Sistema
    /// </summary>
    [Display(Name = "Código de Sistema")]
    [StringLength(10, ErrorMessage = "Código de Sistema deve ter no máximo {1} caracteres")]
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Código de Acoes
    /// </summary>
    [Display(Name = "Código de Acoes")]
    [Required(ErrorMessage = "Código de Acoes é obrigatório")]
    [StringLength(20, ErrorMessage = "Código de Acoes deve ter no máximo {1} caracteres")]
    public string CdAcoes { get; set; } = string.Empty;

    /// <summary>
    /// Código de Restric
    /// </summary>
    [Display(Name = "Código de Restric")]
    [Required(ErrorMessage = "Código de Restric é obrigatório")]
    public string CdRestric { get; set; } = string.Empty;

    /// <summary>
    /// Gr Upodeusuario
    /// </summary>
    [Display(Name = "Gr Upodeusuario")]
    public Guid? IdGrUpodeusuario { get; set; }
}
