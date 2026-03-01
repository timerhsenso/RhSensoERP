// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: VinculoEmpregaticio
// Module: AdministracaoPessoal
// Data: 2026-02-28 21:06:39
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.VinculoEmpregaticio;

/// <summary>
/// Request para criação de Vinculo Empregaticio.
/// Compatível com backend: CreateVinculoEmpregaticioRequest
/// </summary>
public class CreateVinculoEmpregaticioRequest
{
    /// <summary>
    /// Código
    /// </summary>
    [Display(Name = "Código")]
    [StringLength(2, ErrorMessage = "Código deve ter no máximo {1} caracteres")]
    public string Cdvincul { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [StringLength(120, ErrorMessage = "Descrição deve ter no máximo {1} caracteres")]
    public string Dcvincul { get; set; } = string.Empty;

    /// <summary>
    /// Código SEFIP
    /// </summary>
    [Display(Name = "Código SEFIP")]
    [StringLength(2, ErrorMessage = "Código SEFIP deve ter no máximo {1} caracteres")]
    public string Cdsefip { get; set; } = string.Empty;

    /// <summary>
    /// Classe
    /// </summary>
    [Display(Name = "Classe")]
    [StringLength(2, ErrorMessage = "Classe deve ter no máximo {1} caracteres")]
    public string Cdclasse { get; set; } = string.Empty;

    /// <summary>
    /// RAIS
    /// </summary>
    [Display(Name = "RAIS")]
    [Required(ErrorMessage = "RAIS é obrigatório")]
    public int Flrais { get; set; }

    /// <summary>
    /// Natureza Atividade
    /// </summary>
    [Display(Name = "Natureza Atividade")]
    [Required(ErrorMessage = "Natureza Atividade é obrigatório")]
    public int NatativIdade { get; set; }
}
