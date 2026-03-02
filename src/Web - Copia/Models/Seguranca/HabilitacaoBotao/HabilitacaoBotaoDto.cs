// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: HabilitacaoBotao
// Module: Seguranca
// Data: 2026-03-02 17:56:12
// =============================================================================

namespace RhSensoERP.Web.Models.Seguranca.HabilitacaoBotao;

/// <summary>
/// DTO de leitura para Habilitação de Botão.
/// Compatível com backend: RhSensoERP.Modules.Seguranca.Application.DTOs.HabilitacaoBotaoDto
/// </summary>
public class HabilitacaoBotaoDto
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Código de Gr User
    /// </summary>
    public string CdGrUser { get; set; } = string.Empty;

    /// <summary>
    /// Código de Funcao
    /// </summary>
    public string CdFuncao { get; set; } = string.Empty;

    /// <summary>
    /// Código de Sistema
    /// </summary>
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Nome de Botao
    /// </summary>
    public string NmBotao { get; set; } = string.Empty;

    /// <summary>
    /// Gr Upodeusuariofuncao
    /// </summary>
    public Guid? IdGrUpodeusuariofuncao { get; set; }
}
