// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Funcao
// Module: Seguranca
// Data: 2026-03-02 19:25:55
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Seguranca.Funcao;

/// <summary>
/// Request para atualização de Funções do Sistema.
/// Compatível com backend: UpdateFuncaoRequest
/// </summary>
public class UpdateFuncaoRequest
{
    /// <summary>
    /// Descrição de Funcao
    /// </summary>
    [Display(Name = "Descrição de Funcao")]
    [StringLength(80, ErrorMessage = "Descrição de Funcao deve ter no máximo {1} caracteres")]
    public string DcFuncao { get; set; } = string.Empty;

    /// <summary>
    /// Descrição de Modulo
    /// </summary>
    [Display(Name = "Descrição de Modulo")]
    [StringLength(100, ErrorMessage = "Descrição de Modulo deve ter no máximo {1} caracteres")]
    public string DcModulo { get; set; } = string.Empty;

    /// <summary>
    /// Descricao Modulo
    /// </summary>
    [Display(Name = "Descricao Modulo")]
    [StringLength(100, ErrorMessage = "Descricao Modulo deve ter no máximo {1} caracteres")]
    public string Descricaomodulo { get; set; } = string.Empty;
}
