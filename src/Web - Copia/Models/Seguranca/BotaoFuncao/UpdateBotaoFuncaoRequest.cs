// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: BotaoFuncao
// Module: Seguranca
// Data: 2026-03-02 17:53:39
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Seguranca.BotaoFuncao;

/// <summary>
/// Request para atualização de Botões de Função.
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
    public string DcBotao { get; set; } = string.Empty;

    /// <summary>
    /// Código de Acao
    /// </summary>
    [Display(Name = "Código de Acao")]
    [Required(ErrorMessage = "Código de Acao é obrigatório")]
    public string CdAcao { get; set; } = string.Empty;
}
