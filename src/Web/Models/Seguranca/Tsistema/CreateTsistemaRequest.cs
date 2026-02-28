// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Tsistema
// Module: Seguranca
// Data: 2026-02-28 01:05:14
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Seguranca.Tsistema;

/// <summary>
/// Request para criação de Tabela de Sistemas.
/// Compatível com backend: CreateTsistemaRequest
/// </summary>
public class CreateTsistemaRequest
{
    /// <summary>
    /// Cdsi Stema
    /// (Chave Primária - obrigatório na criação)
    /// </summary>
    [Display(Name = "Cdsi Stema")]
    [Required(ErrorMessage = "Cdsi Stema é obrigatório")]
    [StringLength(10, ErrorMessage = "Cdsi Stema deve ter no máximo {1} caracteres")]
    public string CdsiStema { get; set; } = string.Empty;

    /// <summary>
    /// Dcsi Stema
    /// </summary>
    [Display(Name = "Dcsi Stema")]
    [Required(ErrorMessage = "Dcsi Stema é obrigatório")]
    [StringLength(60, ErrorMessage = "Dcsi Stema deve ter no máximo {1} caracteres")]
    public string DcsiStema { get; set; } = string.Empty;
}
