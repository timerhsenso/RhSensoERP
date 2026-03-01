// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Cbo
// Module: AdministracaoPessoal
// Data: 2026-02-28 22:21:22
// =============================================================================

namespace RhSensoERP.Web.Models.AdministracaoPessoal.Cbo;

/// <summary>
/// DTO de leitura para CBO.
/// Compatível com backend: RhSensoERP.Modules.AdministracaoPessoal.Application.DTOs.CboDto
/// </summary>
public class CboDto
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Código CBO
    /// </summary>
    public string Cdcbo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    public string Dccbo { get; set; } = string.Empty;

    /// <summary>
    /// Sinônimos
    /// </summary>
    public string SiNonimo { get; set; } = string.Empty;
}
