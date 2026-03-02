// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TabelaSalarial
// Module: AdministracaoPessoal
// Data: 2026-03-02 18:01:50
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.TabelaSalarial;

/// <summary>
/// Request para atualização de Tabela Salarial.
/// Compatível com backend: UpdateTabelaSalarialRequest
/// </summary>
public class UpdateTabelaSalarialRequest
{
    /// <summary>
    /// Código
    /// </summary>
    [Display(Name = "Código")]
    [StringLength(3, ErrorMessage = "Código deve ter no máximo {1} caracteres")]
    public string CdTabela { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [StringLength(220, ErrorMessage = "Descrição deve ter no máximo {1} caracteres")]
    public string DcTabela { get; set; } = string.Empty;

    /// <summary>
    /// Sequencial
    /// </summary>
    [Display(Name = "Sequencial")]
    [StringLength(1, ErrorMessage = "Sequencial deve ter no máximo {1} caracteres")]
    public string FlSeq { get; set; } = string.Empty;

    /// <summary>
    /// Salário Inicial
    /// </summary>
    [Display(Name = "Salário Inicial")]
    public decimal? VlSalinicial { get; set; }

    /// <summary>
    /// Salário Mediana
    /// </summary>
    [Display(Name = "Salário Mediana")]
    public decimal? VlSalmediana { get; set; }

    /// <summary>
    /// Salário Máximo
    /// </summary>
    [Display(Name = "Salário Máximo")]
    public decimal? VlSalmaximo { get; set; }

    /// <summary>
    /// Data Validade
    /// </summary>
    [Display(Name = "Data Validade")]
    public DateTime? DtValidade { get; set; }

    /// <summary>
    /// Tsalvalidade
    /// </summary>
    [Display(Name = "Tsalvalidade")]
    public Guid? IdTsalvalidade { get; set; }

    /// <summary>
    /// Tsalvalidade Id
    /// </summary>
    [Display(Name = "Tsalvalidade Id")]
    public decimal? TsalvalidadeId { get; set; }
}
