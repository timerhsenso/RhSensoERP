// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: AgenciaBancaria
// Module: AdministracaoPessoal
// Data: 2026-02-28 20:05:58
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.AgenciaBancaria;

/// <summary>
/// Request para criação de Tabela de Agências.
/// Compatível com backend: CreateAgenciaBancariaRequest
/// </summary>
public class CreateAgenciaBancariaRequest
{
    /// <summary>
    /// Código Banco
    /// </summary>
    [Display(Name = "Código Banco")]
    [StringLength(3, ErrorMessage = "Código Banco deve ter no máximo {1} caracteres")]
    public string Cdbanco { get; set; } = string.Empty;

    /// <summary>
    /// Código Agência
    /// </summary>
    [Display(Name = "Código Agência")]
    [StringLength(4, ErrorMessage = "Código Agência deve ter no máximo {1} caracteres")]
    public string Cdagencia { get; set; } = string.Empty;

    /// <summary>
    /// Dígito
    /// </summary>
    [Display(Name = "Dígito")]
    [StringLength(1, ErrorMessage = "Dígito deve ter no máximo {1} caracteres")]
    public string Dvagencia { get; set; } = string.Empty;

    /// <summary>
    /// Nome
    /// </summary>
    [Display(Name = "Nome")]
    [StringLength(40, ErrorMessage = "Nome deve ter no máximo {1} caracteres")]
    public string Nmagencia { get; set; } = string.Empty;

    /// <summary>
    /// Código Município
    /// </summary>
    [Display(Name = "Código Município")]
    [StringLength(5, ErrorMessage = "Código Município deve ter no máximo {1} caracteres")]
    public string Cdmunicip { get; set; } = string.Empty;

    /// <summary>
    /// Conta
    /// </summary>
    [Display(Name = "Conta")]
    [StringLength(15, ErrorMessage = "Conta deve ter no máximo {1} caracteres")]
    public string Noconta { get; set; } = string.Empty;

    /// <summary>
    /// Idbanco
    /// </summary>
    [Display(Name = "Idbanco")]
    public Guid? Idbanco { get; set; }
}
