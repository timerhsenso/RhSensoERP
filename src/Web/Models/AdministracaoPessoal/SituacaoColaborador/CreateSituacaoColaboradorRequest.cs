// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: SituacaoColaborador
// Module: AdministracaoPessoal
// Data: 2026-02-28 22:05:47
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.SituacaoColaborador;

/// <summary>
/// Request para criação de Situação do Colaborador.
/// Compatível com backend: CreateSituacaoColaboradorRequest
/// </summary>
public class CreateSituacaoColaboradorRequest
{
    /// <summary>
    /// Código
    /// </summary>
    [Display(Name = "Código")]
    [StringLength(2, ErrorMessage = "Código deve ter no máximo {1} caracteres")]
    public string Cdsituacao { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [StringLength(40, ErrorMessage = "Descrição deve ter no máximo {1} caracteres")]
    public string Dcsituacao { get; set; } = string.Empty;

    /// <summary>
    /// É Demissão
    /// </summary>
    [Display(Name = "É Demissão")]
    [StringLength(1, ErrorMessage = "É Demissão deve ter no máximo {1} caracteres")]
    public string Fldemissao { get; set; } = string.Empty;

    /// <summary>
    /// É Afastamento
    /// </summary>
    [Display(Name = "É Afastamento")]
    [StringLength(1, ErrorMessage = "É Afastamento deve ter no máximo {1} caracteres")]
    public string FlafaStame { get; set; } = string.Empty;

    /// <summary>
    /// Dias Benefício
    /// </summary>
    [Display(Name = "Dias Benefício")]
    public int? Qtdiasbene { get; set; }

    /// <summary>
    /// Dias Previdência
    /// </summary>
    [Display(Name = "Dias Previdência")]
    public int? Qtdiasprev { get; set; }

    /// <summary>
    /// Código FGTS
    /// </summary>
    [Display(Name = "Código FGTS")]
    [StringLength(1, ErrorMessage = "Código FGTS deve ter no máximo {1} caracteres")]
    public string Cdfgts { get; set; } = string.Empty;

    /// <summary>
    /// Código SEFIP
    /// </summary>
    [Display(Name = "Código SEFIP")]
    [StringLength(1, ErrorMessage = "Código SEFIP deve ter no máximo {1} caracteres")]
    public string Cdsefip { get; set; } = string.Empty;

    /// <summary>
    /// Código SEFIP 2
    /// </summary>
    [Display(Name = "Código SEFIP 2")]
    [StringLength(2, ErrorMessage = "Código SEFIP 2 deve ter no máximo {1} caracteres")]
    public string Cdsefip2 { get; set; } = string.Empty;

    /// <summary>
    /// Perde Férias
    /// </summary>
    [Display(Name = "Perde Férias")]
    [StringLength(1, ErrorMessage = "Perde Férias deve ter no máximo {1} caracteres")]
    public string Flpferias { get; set; } = string.Empty;
}
