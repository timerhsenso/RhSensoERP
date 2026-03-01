// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TabelaSalarial
// Module: AdministracaoPessoal
// Data: 2026-02-28 22:03:43
// =============================================================================

namespace RhSensoERP.Web.Models.AdministracaoPessoal.TabelaSalarial;

/// <summary>
/// DTO de leitura para Tabela Salarial.
/// Compatível com backend: RhSensoERP.Modules.AdministracaoPessoal.Application.DTOs.TabelaSalarialDto
/// </summary>
public class TabelaSalarialDto
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Código
    /// </summary>
    public string Cdtabela { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    public string Dctabela { get; set; } = string.Empty;

    /// <summary>
    /// Sequencial
    /// </summary>
    public string Flseq { get; set; } = string.Empty;

    /// <summary>
    /// Salário Inicial
    /// </summary>
    public decimal? Vlsalinicial { get; set; }

    /// <summary>
    /// Salário Mediana
    /// </summary>
    public decimal? Vlsalmediana { get; set; }

    /// <summary>
    /// Salário Máximo
    /// </summary>
    public decimal? Vlsalmaximo { get; set; }

    /// <summary>
    /// Data Validade
    /// </summary>
    public DateTime? DtvalIdade { get; set; }

    /// <summary>
    /// Idtsalvalidade
    /// </summary>
    public Guid? IdtsalvalIdade { get; set; }

    /// <summary>
    /// Tsalval Idade Id
    /// </summary>
    public decimal? TsalvalIdadeId { get; set; }
}
