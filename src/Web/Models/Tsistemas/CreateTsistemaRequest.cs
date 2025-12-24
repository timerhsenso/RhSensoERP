// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.1
// Entity: Tsistema
// Data: 2025-12-02 02:25:04
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Tsistemas;

/// <summary>
/// Request para criação de Tsistema.
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
