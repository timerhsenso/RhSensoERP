// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: HabilitacaoBotao
// Module: Seguranca
// Data: 2026-03-01 13:35:46
// =============================================================================

namespace RhSensoERP.Web.Models.Seguranca.HabilitacaoBotao;

/// <summary>
/// DTO de leitura para Hab botao.
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
    public string Cdgruser { get; set; } = string.Empty;

    /// <summary>
    /// Código de Funcao
    /// </summary>
    public string Cdfuncao { get; set; } = string.Empty;

    /// <summary>
    /// Código de Sistema
    /// </summary>
    public string CdsiStema { get; set; } = string.Empty;

    /// <summary>
    /// Nome de Botao
    /// </summary>
    public string Nmbotao { get; set; } = string.Empty;

    /// <summary>
    /// Idgrupodeusuariofuncao
    /// </summary>
    public Guid? Idgrupodeusuariofuncao { get; set; }
}
