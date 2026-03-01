// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Municipio
// Module: AdministracaoPessoal
// Data: 2026-02-28 22:07:57
// =============================================================================

namespace RhSensoERP.Web.Models.AdministracaoPessoal.Municipio;

/// <summary>
/// DTO de leitura para Municipios.
/// Compatível com backend: RhSensoERP.Modules.AdministracaoPessoal.Application.DTOs.MunicipioDto
/// </summary>
public class MunicipioDto
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Código
    /// </summary>
    public string Cdmunicip { get; set; } = string.Empty;

    /// <summary>
    /// UF
    /// </summary>
    public string SgeStado { get; set; } = string.Empty;

    /// <summary>
    /// Nome
    /// </summary>
    public string Nmmunicip { get; set; } = string.Empty;

    /// <summary>
    /// Cod Ibge
    /// </summary>
    public int? CodIbge { get; set; }
}
