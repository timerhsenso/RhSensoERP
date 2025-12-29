// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: An1
// Module: GestaoDePessoas
// Data: 2025-12-28 21:00:16
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.GestaoDePessoas.An1;

/// <summary>
/// Request para atualizacao de Banco.
/// Compativel com backend: UpdateAn1Request
/// </summary>
public class UpdateAn1Request
{
    /// <summary>
    /// Cdbanco
    /// </summary>
    [Display(Name = "Cdbanco")]
    [StringLength(3, ErrorMessage = "Cdbanco deve ter no maximo {1} caracteres")]
    public string Cdbanco { get; set; } = string.Empty;

    /// <summary>
    /// Dcbanco
    /// </summary>
    [Display(Name = "Dcbanco")]
    [StringLength(40, ErrorMessage = "Dcbanco deve ter no maximo {1} caracteres")]
    public string Dcbanco { get; set; } = string.Empty;
}
