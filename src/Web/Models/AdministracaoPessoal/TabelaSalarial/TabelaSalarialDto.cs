// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TabelaSalarial
// Module: AdministracaoPessoal
// Data: 2026-03-02 18:01:50
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
    public string CdTabela { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    public string DcTabela { get; set; } = string.Empty;

    /// <summary>
    /// Sequencial
    /// </summary>
    public string FlSeq { get; set; } = string.Empty;

    /// <summary>
    /// Salário Inicial
    /// </summary>
    public decimal? VlSalinicial { get; set; }

    /// <summary>
    /// Salário Mediana
    /// </summary>
    public decimal? VlSalmediana { get; set; }

    /// <summary>
    /// Salário Máximo
    /// </summary>
    public decimal? VlSalmaximo { get; set; }

    /// <summary>
    /// Data Validade
    /// </summary>
    public DateTime? DtValidade { get; set; }

    /// <summary>
    /// Tsalvalidade
    /// </summary>
    public Guid? IdTsalvalidade { get; set; }

    /// <summary>
    /// Tsalvalidade Id
    /// </summary>
    public decimal? TsalvalidadeId { get; set; }
}
