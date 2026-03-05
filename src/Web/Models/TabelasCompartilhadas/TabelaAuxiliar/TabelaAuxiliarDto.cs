// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TabelaAuxiliar
// Module: TabelasCompartilhadas
// Data: 2026-03-04 16:27:03
// =============================================================================

namespace RhSensoERP.Web.Models.TabelasCompartilhadas.TabelaAuxiliar;

/// <summary>
/// DTO de leitura para Tabela Auxiliar.
/// Compatível com backend: RhSensoERP.Modules.TabelasCompartilhadas.Application.DTOs.TabelaAuxiliarDto
/// </summary>
public class TabelaAuxiliarDto
{
    /// <summary>
    /// Código Tipo Tabela
    /// </summary>
    public string CdTpTabela { get; set; } = string.Empty;

    /// <summary>
    /// Código Situação
    /// </summary>
    public string CdSituacao { get; set; } = string.Empty;

    /// <summary>
    /// Descrição Situação
    /// </summary>
    public string DcSituacao { get; set; } = string.Empty;

    /// <summary>
    /// Ordem
    /// </summary>
    public int? NoOrdem { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    public string FlAtivoaux { get; set; } = string.Empty;
}
