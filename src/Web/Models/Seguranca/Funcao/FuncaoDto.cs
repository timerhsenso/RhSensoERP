// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Funcao
// Module: Seguranca
// Data: 2026-02-28 19:04:06
// =============================================================================

namespace RhSensoERP.Web.Models.Seguranca.Funcao;

/// <summary>
/// DTO de leitura para Funções do Sistema.
/// Compatível com backend: RhSensoERP.Modules.Seguranca.Application.DTOs.FuncaoDto
/// </summary>
public class FuncaoDto
{
    /// <summary>
    /// Código de Funcao
    /// </summary>
    public string Cdfuncao { get; set; } = string.Empty;

    /// <summary>
    /// Código de Sistema
    /// </summary>
    public string CdsiStema { get; set; } = string.Empty;

    /// <summary>
    /// Descrição de Funcao
    /// </summary>
    public string Dcfuncao { get; set; } = string.Empty;

    /// <summary>
    /// Descrição de Modulo
    /// </summary>
    public string Dcmodulo { get; set; } = string.Empty;

    /// <summary>
    /// Descricao Modulo
    /// </summary>
    public string Descricaomodulo { get; set; } = string.Empty;
}
