// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TabelaAuxiliar
// Module: TabelasCompartilhadas
// Data: 2026-03-04 16:27:03
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.TabelasCompartilhadas.TabelaAuxiliar;

/// <summary>
/// Request para atualização de Tabela Auxiliar.
/// Compatível com backend: UpdateTabelaAuxiliarRequest
/// </summary>
public class UpdateTabelaAuxiliarRequest
{
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
