// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: BotaoFuncao
// Module: Seguranca
// Data: 2026-02-28 19:22:44
// =============================================================================

namespace RhSensoERP.Web.Models.Seguranca.BotaoFuncao;

/// <summary>
/// DTO de leitura para Tabela de Botões.
/// Compatível com backend: RhSensoERP.Modules.Seguranca.Application.DTOs.BotaoFuncaoDto
/// </summary>
public class BotaoFuncaoDto
{
    /// <summary>
    /// Código de Sistema
    /// </summary>
    public string CdsiStema { get; set; } = string.Empty;

    /// <summary>
    /// Código de Funcao
    /// </summary>
    public string Cdfuncao { get; set; } = string.Empty;

    /// <summary>
    /// Nome de Botao
    /// </summary>
    public string Nmbotao { get; set; } = string.Empty;

    /// <summary>
    /// Descrição de Botao
    /// </summary>
    public string Dcbotao { get; set; } = string.Empty;

    /// <summary>
    /// Código de Acao
    /// </summary>
    public string Cdacao { get; set; } = string.Empty;
}
