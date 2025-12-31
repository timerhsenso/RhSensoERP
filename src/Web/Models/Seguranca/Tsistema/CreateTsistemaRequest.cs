// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: Tsistema
// Module: Seguranca
// Data: 2025-12-30 17:48:37
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Seguranca.Tsistema;

/// <summary>
/// Request para criacao de Tabela de Sistemas.
/// Compativel com backend: CreateTsistemaRequest
/// </summary>
public class CreateTsistemaRequest
{
    /// <summary>
    /// CdsiStema
    /// (Chave Primaria - obrigatorio na criacao)
    /// </summary>
    [Display(Name = "CdsiStema")]
    [Required(ErrorMessage = "CdsiStema e obrigatorio")]
    [StringLength(10, ErrorMessage = "CdsiStema deve ter no maximo {1} caracteres")]
    public string CdsiStema { get; set; } = string.Empty;

    /// <summary>
    /// DcsiStema
    /// </summary>
    [Display(Name = "DcsiStema")]
    [Required(ErrorMessage = "DcsiStema e obrigatorio")]
    [StringLength(60, ErrorMessage = "DcsiStema deve ter no maximo {1} caracteres")]
    public string DcsiStema { get; set; } = string.Empty;
}
