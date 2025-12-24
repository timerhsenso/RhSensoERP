// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: Tvin1
// Module: GestaoDePessoas
// Data: 2025-12-24 00:36:02
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.GestaoDePessoas.Tvin1;

/// <summary>
/// Request para atualização de Vínculo Empregatício.
/// Compatível com backend: UpdateTvin1Request
/// </summary>
public class UpdateTvin1Request
{
    /// <summary>
    /// Cdvincul
    /// </summary>
    [Display(Name = "Cdvincul")]
    [StringLength(2, ErrorMessage = "Cdvincul deve ter no máximo {1} caracteres")]
    public string Cdvincul { get; set; } = string.Empty;

    /// <summary>
    /// Dcvincul
    /// </summary>
    [Display(Name = "Dcvincul")]
    [StringLength(120, ErrorMessage = "Dcvincul deve ter no máximo {1} caracteres")]
    public string Dcvincul { get; set; } = string.Empty;

    /// <summary>
    /// Cdsefip
    /// </summary>
    [Display(Name = "Cdsefip")]
    [StringLength(2, ErrorMessage = "Cdsefip deve ter no máximo {1} caracteres")]
    public string Cdsefip { get; set; } = string.Empty;

    /// <summary>
    /// Cdclasse
    /// </summary>
    [Display(Name = "Cdclasse")]
    [StringLength(2, ErrorMessage = "Cdclasse deve ter no máximo {1} caracteres")]
    public string Cdclasse { get; set; } = string.Empty;

    /// <summary>
    /// Flrais
    /// </summary>
    [Display(Name = "Flrais")]
    [Required(ErrorMessage = "Flrais é obrigatório")]
    public int Flrais { get; set; }

    /// <summary>
    /// Natatividade
    /// </summary>
    [Display(Name = "Natatividade")]
    [Required(ErrorMessage = "Natatividade é obrigatório")]
    public int NatativIdade { get; set; }
}
