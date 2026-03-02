// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: VinculoEmpregaticio
// Module: AdministracaoPessoal
// Data: 2026-03-02 18:00:36
// =============================================================================

namespace RhSensoERP.Web.Models.AdministracaoPessoal.VinculoEmpregaticio;

/// <summary>
/// DTO de leitura para Vínculo Empregatício.
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
    public string CdVincul { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    public string DcVincul { get; set; } = string.Empty;

    /// <summary>
    /// Código SEFIP
    /// </summary>
    public string CdSefip { get; set; } = string.Empty;

    /// <summary>
    /// Classe
    /// </summary>
    public string CdClasse { get; set; } = string.Empty;

    /// <summary>
    /// RAIS
    /// </summary>
    public int FlRais { get; set; }

    /// <summary>
    /// Natureza Atividade
    /// </summary>
    public int Natatividade { get; set; }
}
