// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: VinculoEmpregaticio
// Module: AdministracaoPessoal
// Data: 2026-03-02 18:00:36
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.VinculoEmpregaticio;

/// <summary>
/// Request para atualização de Vínculo Empregatício.
/// Compatível com backend: UpdateVinculoEmpregaticioRequest
/// </summary>
public class UpdateVinculoEmpregaticioRequest
{
    /// <summary>
    /// Código
    /// </summary>
    [Display(Name = "Código")]
    [StringLength(2, ErrorMessage = "Código deve ter no máximo {1} caracteres")]
    public string CdVincul { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [StringLength(120, ErrorMessage = "Descrição deve ter no máximo {1} caracteres")]
    public string DcVincul { get; set; } = string.Empty;

    /// <summary>
    /// Código SEFIP
    /// </summary>
    [Display(Name = "Código SEFIP")]
    [StringLength(2, ErrorMessage = "Código SEFIP deve ter no máximo {1} caracteres")]
    public string CdSefip { get; set; } = string.Empty;

    /// <summary>
    /// Classe
    /// </summary>
    [Display(Name = "Classe")]
    [StringLength(2, ErrorMessage = "Classe deve ter no máximo {1} caracteres")]
    public string CdClasse { get; set; } = string.Empty;

    /// <summary>
    /// RAIS
    /// </summary>
    [Display(Name = "RAIS")]
    [Required(ErrorMessage = "RAIS é obrigatório")]
    public int FlRais { get; set; }

    /// <summary>
    /// Natureza Atividade
    /// </summary>
    [Display(Name = "Natureza Atividade")]
    [Required(ErrorMessage = "Natureza Atividade é obrigatório")]
    public int Natatividade { get; set; }
}
