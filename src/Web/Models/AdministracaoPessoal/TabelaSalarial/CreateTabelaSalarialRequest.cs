// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TabelaSalarial
// Module: AdministracaoPessoal
// Data: 2026-02-28 22:03:43
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.TabelaSalarial;

/// <summary>
/// Request para criação de Tabela Salarial.
/// Compatível com backend: CreateTabelaSalarialRequest
/// </summary>
public class CreateTabelaSalarialRequest
{
    /// <summary>
    /// Código
    /// </summary>
    [Display(Name = "Código")]
    [StringLength(3, ErrorMessage = "Código deve ter no máximo {1} caracteres")]
    public string Cdtabela { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [StringLength(220, ErrorMessage = "Descrição deve ter no máximo {1} caracteres")]
    public string Dctabela { get; set; } = string.Empty;

    /// <summary>
    /// Sequencial
    /// </summary>
    [Display(Name = "Sequencial")]
    [StringLength(1, ErrorMessage = "Sequencial deve ter no máximo {1} caracteres")]
    public string Flseq { get; set; } = string.Empty;

    /// <summary>
    /// Salário Inicial
    /// </summary>
    [Display(Name = "Salário Inicial")]
    public decimal? Vlsalinicial { get; set; }

    /// <summary>
    /// Salário Mediana
    /// </summary>
    [Display(Name = "Salário Mediana")]
    public decimal? Vlsalmediana { get; set; }

    /// <summary>
    /// Salário Máximo
    /// </summary>
    [Display(Name = "Salário Máximo")]
    public decimal? Vlsalmaximo { get; set; }

    /// <summary>
    /// Data Validade
    /// </summary>
    [Display(Name = "Data Validade")]
    public DateTime? DtvalIdade { get; set; }

    /// <summary>
    /// Idtsalvalidade
    /// </summary>
    [Display(Name = "Idtsalvalidade")]
    public Guid? IdtsalvalIdade { get; set; }

    /// <summary>
    /// Tsalval Idade Id
    /// </summary>
    [Display(Name = "Tsalval Idade Id")]
    public decimal? TsalvalIdadeId { get; set; }
}
