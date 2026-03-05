// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TipoTabelaAuxiliar
// Module: TabelasCompartilhadas
// Data: 2026-03-04 00:11:12
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.TabelasCompartilhadas.TipoTabelaAuxiliar;

/// <summary>
/// Request para criação de Tabela Auxiliar.
/// Compatível com backend: CreateTipoTabelaAuxiliarRequest
/// </summary>
public class CreateTipoTabelaAuxiliarRequest
{
    /// <summary>
    /// Código de Tp Tabela
    /// (Chave Primária - obrigatório na criação)
    /// </summary>
    [Display(Name = "Código de Tp Tabela")]
    [Required(ErrorMessage = "Código de Tp Tabela é obrigatório")]
    [StringLength(2, ErrorMessage = "Código de Tp Tabela deve ter no máximo {1} caracteres")]
    public string CdTpTabela { get; set; } = string.Empty;

    /// <summary>
    /// Descrição de Tabela
    /// </summary>
    [Display(Name = "Descrição de Tabela")]
    [Required(ErrorMessage = "Descrição de Tabela é obrigatório")]
    [StringLength(60, ErrorMessage = "Descrição de Tabela deve ter no máximo {1} caracteres")]
    public string DcTabela { get; set; } = string.Empty;
}
