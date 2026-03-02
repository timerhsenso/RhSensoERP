// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: BotaoFuncao
// Module: Seguranca
// Data: 2026-03-02 17:53:39
// =============================================================================

namespace RhSensoERP.Web.Models.Seguranca.BotaoFuncao;

/// <summary>
/// DTO de leitura para Botões de Função.
/// Compatível com backend: RhSensoERP.Modules.Seguranca.Application.DTOs.BotaoFuncaoDto
/// </summary>
public class BotaoFuncaoDto
{
    /// <summary>
    /// Código de Sistema
    /// </summary>
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Código de Funcao
    /// </summary>
    public string CdFuncao { get; set; } = string.Empty;

    /// <summary>
    /// Nome de Botao
    /// </summary>
    public string NmBotao { get; set; } = string.Empty;

    /// <summary>
    /// Descrição de Botao
    /// </summary>
    public string DcBotao { get; set; } = string.Empty;

    /// <summary>
    /// Código de Acao
    /// </summary>
    public string CdAcao { get; set; } = string.Empty;
}
