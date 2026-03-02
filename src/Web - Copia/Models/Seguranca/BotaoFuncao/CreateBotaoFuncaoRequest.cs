// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: BotaoFuncao
// Module: Seguranca
// Data: 2026-03-02 17:53:39
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Seguranca.BotaoFuncao;

/// <summary>
/// Request para criação de Botões de Função.
/// Compatível com backend: CreateBotaoFuncaoRequest
/// </summary>
public class CreateBotaoFuncaoRequest
{
    /// <summary>
    /// Código de Sistema
    /// (Chave Primária - obrigatório na criação)
    /// </summary>
    [Display(Name = "Código de Sistema")]
    [Required(ErrorMessage = "Código de Sistema é obrigatório")]
    [StringLength(10, ErrorMessage = "Código de Sistema deve ter no máximo {1} caracteres")]
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Código de Funcao
    /// (Chave Primária - obrigatório na criação)
    /// </summary>
    [Display(Name = "Código de Funcao")]
    [Required(ErrorMessage = "Código de Funcao é obrigatório")]
    [StringLength(30, ErrorMessage = "Código de Funcao deve ter no máximo {1} caracteres")]
    public string CdFuncao { get; set; } = string.Empty;

    /// <summary>
    /// Nome de Botao
    /// (Chave Primária - obrigatório na criação)
    /// </summary>
    [Display(Name = "Nome de Botao")]
    [Required(ErrorMessage = "Nome de Botao é obrigatório")]
    [StringLength(30, ErrorMessage = "Nome de Botao deve ter no máximo {1} caracteres")]
    public string NmBotao { get; set; } = string.Empty;

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
