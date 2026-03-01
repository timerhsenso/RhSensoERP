// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: GrauInstrucao
// Module: AdministracaoPessoal
// Data: 2026-02-28 19:50:42
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.GrauInstrucao;

/// <summary>
/// Request para atualização de Grau de instrucao.
/// Compatível com backend: UpdateGrauInstrucaoRequest
/// </summary>
public class UpdateGrauInstrucaoRequest
{
    /// <summary>
    /// Código
    /// </summary>
    [Display(Name = "Código")]
    [StringLength(2, ErrorMessage = "Código deve ter no máximo {1} caracteres")]
    public string CdinStruc { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [StringLength(40, ErrorMessage = "Descrição deve ter no máximo {1} caracteres")]
    public string DcinStruc { get; set; } = string.Empty;

    /// <summary>
    /// Código RAIS
    /// </summary>
    [Display(Name = "Código RAIS")]
    [StringLength(2, ErrorMessage = "Código RAIS deve ter no máximo {1} caracteres")]
    public string Cdrais { get; set; } = string.Empty;

    /// <summary>
    /// Código CAGED
    /// </summary>
    [Display(Name = "Código CAGED")]
    [StringLength(2, ErrorMessage = "Código CAGED deve ter no máximo {1} caracteres")]
    public string Cdcaged { get; set; } = string.Empty;

    /// <summary>
    /// Código eSocial
    /// </summary>
    [Display(Name = "Código eSocial")]
    [StringLength(2, ErrorMessage = "Código eSocial deve ter no máximo {1} caracteres")]
    public string Cdesocial { get; set; } = string.Empty;
}
