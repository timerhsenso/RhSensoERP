// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: GrauInstrucao
// Module: AdministracaoPessoal
// Data: 2026-02-28 19:50:42
// =============================================================================

namespace RhSensoERP.Web.Models.AdministracaoPessoal.GrauInstrucao;

/// <summary>
/// DTO de leitura para Grau de instrucao.
/// Compatível com backend: RhSensoERP.Modules.AdministracaoPessoal.Application.DTOs.GrauInstrucaoDto
/// </summary>
public class GrauInstrucaoDto
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Código
    /// </summary>
    public string CdinStruc { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    public string DcinStruc { get; set; } = string.Empty;

    /// <summary>
    /// Código RAIS
    /// </summary>
    public string Cdrais { get; set; } = string.Empty;

    /// <summary>
    /// Código CAGED
    /// </summary>
    public string Cdcaged { get; set; } = string.Empty;

    /// <summary>
    /// Código eSocial
    /// </summary>
    public string Cdesocial { get; set; } = string.Empty;
}
