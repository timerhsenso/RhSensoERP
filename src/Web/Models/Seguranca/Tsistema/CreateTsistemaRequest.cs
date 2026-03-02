// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Tsistema
// Module: Seguranca
// Data: 2026-03-01 16:07:47
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
    /// Código de Si Stema
    /// (Chave Primária - obrigatório na criação)
    /// </summary>
    [Display(Name = "Código de Si Stema")]
    [Required(ErrorMessage = "Código de Si Stema é obrigatório")]
    [StringLength(10, ErrorMessage = "Código de Si Stema deve ter no máximo {1} caracteres")]
    public string CdsiStema { get; set; } = string.Empty;

    /// <summary>
    /// Descrição de Si Stema
    /// </summary>
    [Display(Name = "Descrição de Si Stema")]
    [Required(ErrorMessage = "Descrição de Si Stema é obrigatório")]
    [StringLength(60, ErrorMessage = "Descrição de Si Stema deve ter no máximo {1} caracteres")]
    public string DcsiStema { get; set; } = string.Empty;
}
