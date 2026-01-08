// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Tsistema
// Module: Identity
// Data: 2026-01-07 22:17:31
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Identity.Tsistema;

/// <summary>
/// Request para atualização de Tabela de Sistemas.
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
