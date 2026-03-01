// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Municipio
// Module: AdministracaoPessoal
// Data: 2026-02-28 22:07:57
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.Municipio;

/// <summary>
/// Request para atualização de Municipios.
/// Compatível com backend: UpdateMunicipioRequest
/// </summary>
public class UpdateMunicipioRequest
{
    /// <summary>
    /// Código
    /// </summary>
    [Display(Name = "Código")]
    [StringLength(5, ErrorMessage = "Código deve ter no máximo {1} caracteres")]
    public string Cdmunicip { get; set; } = string.Empty;

    /// <summary>
    /// UF
    /// </summary>
    [Display(Name = "UF")]
    [StringLength(2, ErrorMessage = "UF deve ter no máximo {1} caracteres")]
    public string SgeStado { get; set; } = string.Empty;

    /// <summary>
    /// Nome
    /// </summary>
    [Display(Name = "Nome")]
    [StringLength(60, ErrorMessage = "Nome deve ter no máximo {1} caracteres")]
    public string Nmmunicip { get; set; } = string.Empty;

    /// <summary>
    /// Cod Ibge
    /// </summary>
    [Display(Name = "Cod Ibge")]
    public int? CodIbge { get; set; }
}
