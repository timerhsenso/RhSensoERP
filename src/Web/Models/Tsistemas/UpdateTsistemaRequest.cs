// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.1
// Entity: Tsistema
// Data: 2025-12-02 02:25:04
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Tsistemas;

/// <summary>
/// Request para atualização de Tsistema.
/// Compatível com backend: UpdateTsistemaRequest
/// </summary>
public class UpdateTsistemaRequest
{
    /// <summary>
    /// Dcsi Stema
    /// </summary>
    [Display(Name = "Dcsi Stema")]
    [Required(ErrorMessage = "Dcsi Stema é obrigatório")]
    [StringLength(60, ErrorMessage = "Dcsi Stema deve ter no máximo {1} caracteres")]
    public string DcsiStema { get; set; } = string.Empty;
}
