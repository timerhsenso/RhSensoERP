// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Cbo
// Module: AdministracaoPessoal
// Data: 2026-02-28 22:21:22
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.Cbo;

/// <summary>
/// Request para criação de CBO.
/// Compatível com backend: CreateCboRequest
/// </summary>
public class CreateCboRequest
{
    /// <summary>
    /// Código CBO
    /// </summary>
    [Display(Name = "Código CBO")]
    [StringLength(6, ErrorMessage = "Código CBO deve ter no máximo {1} caracteres")]
    public string Cdcbo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [StringLength(80, ErrorMessage = "Descrição deve ter no máximo {1} caracteres")]
    public string Dccbo { get; set; } = string.Empty;

    /// <summary>
    /// Sinônimos
    /// </summary>
    [Display(Name = "Sinônimos")]
    [StringLength(4000, ErrorMessage = "Sinônimos deve ter no máximo {1} caracteres")]
    public string SiNonimo { get; set; } = string.Empty;
}
