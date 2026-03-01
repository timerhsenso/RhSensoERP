// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: VinculoEmpregaticio
// Module: AdministracaoPessoal
// Data: 2026-02-28 21:06:39
// =============================================================================

namespace RhSensoERP.Web.Models.AdministracaoPessoal.VinculoEmpregaticio;

/// <summary>
/// DTO de leitura para Vinculo Empregaticio.
/// Compatível com backend: RhSensoERP.Modules.AdministracaoPessoal.Application.DTOs.VinculoEmpregaticioDto
/// </summary>
public class VinculoEmpregaticioDto
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Código
    /// </summary>
    public string Cdvincul { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    public string Dcvincul { get; set; } = string.Empty;

    /// <summary>
    /// Código SEFIP
    /// </summary>
    public string Cdsefip { get; set; } = string.Empty;

    /// <summary>
    /// Classe
    /// </summary>
    public string Cdclasse { get; set; } = string.Empty;

    /// <summary>
    /// RAIS
    /// </summary>
    public int Flrais { get; set; }

    /// <summary>
    /// Natureza Atividade
    /// </summary>
    public int NatativIdade { get; set; }
}
