// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Tsistema
// Module: Seguranca
// Data: 2026-02-28 19:16:10
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Seguranca.Tsistema;

/// <summary>
/// Request para atualização de Tabela de Sistemas.
/// Compatível com backend: UpdateTsistemaRequest
/// </summary>
public class UpdateTsistemaRequest
{
    /// <summary>
    /// Descrição de Si Stema
    /// </summary>
    [Display(Name = "Descrição de Si Stema")]
    [Required(ErrorMessage = "Descrição de Si Stema é obrigatório")]
    [StringLength(60, ErrorMessage = "Descrição de Si Stema deve ter no máximo {1} caracteres")]
    public string DcsiStema { get; set; } = string.Empty;
}
