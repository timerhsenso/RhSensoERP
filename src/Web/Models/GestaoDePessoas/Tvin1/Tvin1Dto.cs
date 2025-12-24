// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: Tvin1
// Module: GestaoDePessoas
// Data: 2025-12-24 00:36:02
// =============================================================================

namespace RhSensoERP.Web.Models.GestaoDePessoas.Tvin1;

/// <summary>
/// DTO de leitura para Vínculo Empregatício.
/// Compatível com backend: RhSensoERP.Modules.GestaoDePessoas.Application.DTOs.Tvin1Dto
/// </summary>
public class Tvin1Dto
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Cdvincul
    /// </summary>
    public string Cdvincul { get; set; } = string.Empty;

    /// <summary>
    /// Dcvincul
    /// </summary>
    public string Dcvincul { get; set; } = string.Empty;

    /// <summary>
    /// Cdsefip
    /// </summary>
    public string Cdsefip { get; set; } = string.Empty;

    /// <summary>
    /// Cdclasse
    /// </summary>
    public string Cdclasse { get; set; } = string.Empty;

    /// <summary>
    /// Flrais
    /// </summary>
    public int Flrais { get; set; }

    /// <summary>
    /// Natatividade
    /// </summary>
    public int NatativIdade { get; set; }
}
