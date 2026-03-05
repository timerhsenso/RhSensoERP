// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TabelaAuxiliar
// Module: TabelasCompartilhadas
// Data: 2026-03-04 16:27:03
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.TabelasCompartilhadas.TabelaAuxiliar;

/// <summary>
/// Request para criação de Tabela Auxiliar.
/// Compatível com backend: CreateTabelaAuxiliarRequest
/// </summary>
public class CreateTabelaAuxiliarRequest
{
    /// <summary>
    /// Código Tipo Tabela
    /// (Chave Primária - obrigatório na criação)
    /// </summary>
    [Display(Name = "Código Tipo Tabela")]
    [Required(ErrorMessage = "Código Tipo Tabela é obrigatório")]
    [StringLength(2, ErrorMessage = "Código Tipo Tabela deve ter no máximo {1} caracteres")]
    public string CdTpTabela { get; set; } = string.Empty;

    /// <summary>
    /// Código Situação
    /// (Chave Primária - obrigatório na criação)
    /// </summary>
    [Display(Name = "Código Situação")]
    [Required(ErrorMessage = "Código Situação é obrigatório")]
    [StringLength(2, ErrorMessage = "Código Situação deve ter no máximo {1} caracteres")]
    public string CdSituacao { get; set; } = string.Empty;

    /// <summary>
    /// Descrição Situação
    /// </summary>
    [Display(Name = "Descrição Situação")]
    [Required(ErrorMessage = "Descrição Situação é obrigatório")]
    [StringLength(60, ErrorMessage = "Descrição Situação deve ter no máximo {1} caracteres")]
    public string DcSituacao { get; set; } = string.Empty;

    /// <summary>
    /// Ordem
    /// </summary>
    [Display(Name = "Ordem")]
    public int? NoOrdem { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [StringLength(1, ErrorMessage = "Ativo deve ter no máximo {1} caracteres")]
    public string FlAtivoaux { get; set; } = string.Empty;
}
