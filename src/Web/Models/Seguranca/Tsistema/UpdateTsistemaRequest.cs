// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: Tsistema
// Module: Seguranca
// Data: 2025-12-30 17:48:37
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Seguranca.Tsistema;

/// <summary>
/// Request para atualizacao de Tabela de Sistemas.
/// Compativel com backend: UpdateTsistemaRequest
/// </summary>
public class UpdateTsistemaRequest
{
    /// <summary>
    /// DcsiStema
    /// </summary>
    [Display(Name = "DcsiStema")]
    [Required(ErrorMessage = "DcsiStema e obrigatorio")]
    [StringLength(60, ErrorMessage = "DcsiStema deve ter no maximo {1} caracteres")]
    public string DcsiStema { get; set; } = string.Empty;
}
