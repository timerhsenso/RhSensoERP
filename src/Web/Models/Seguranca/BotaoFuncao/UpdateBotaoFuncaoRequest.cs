// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: BotaoFuncao
// Module: Seguranca
// Data: 2026-02-28 19:22:44
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Seguranca.BotaoFuncao;

/// <summary>
/// Request para atualização de Tabela de Botões.
/// Compatível com backend: UpdateBotaoFuncaoRequest
/// </summary>
public class UpdateBotaoFuncaoRequest
{
    /// <summary>
    /// Descrição de Botao
    /// </summary>
    [Display(Name = "Descrição de Botao")]
    [Required(ErrorMessage = "Descrição de Botao é obrigatório")]
    [StringLength(60, ErrorMessage = "Descrição de Botao deve ter no máximo {1} caracteres")]
    public string Dcbotao { get; set; } = string.Empty;

    /// <summary>
    /// Código de Acao
    /// </summary>
    [Display(Name = "Código de Acao")]
    [Required(ErrorMessage = "Código de Acao é obrigatório")]
    public string Cdacao { get; set; } = string.Empty;
}
