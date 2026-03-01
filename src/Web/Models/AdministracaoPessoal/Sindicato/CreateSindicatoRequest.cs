// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Sindicato
// Module: AdministracaoPessoal
// Data: 2026-02-28 21:56:09
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.Sindicato;

/// <summary>
/// Request para criação de Sindicato.
/// Compatível com backend: CreateSindicatoRequest
/// </summary>
public class CreateSindicatoRequest
{
    /// <summary>
    /// Código
    /// </summary>
    [Display(Name = "Código")]
    [StringLength(2, ErrorMessage = "Código deve ter no máximo {1} caracteres")]
    public string Cdsindicat { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [StringLength(40, ErrorMessage = "Descrição deve ter no máximo {1} caracteres")]
    public string Dcsindicat { get; set; } = string.Empty;

    /// <summary>
    /// Endereço
    /// </summary>
    [Display(Name = "Endereço")]
    [StringLength(30, ErrorMessage = "Endereço deve ter no máximo {1} caracteres")]
    public string Dcendereco { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ
    /// </summary>
    [Display(Name = "CNPJ")]
    [StringLength(14, ErrorMessage = "CNPJ deve ter no máximo {1} caracteres")]
    public string Cgcsindicat { get; set; } = string.Empty;

    /// <summary>
    /// Código Entidade
    /// </summary>
    [Display(Name = "Código Entidade")]
    [StringLength(20, ErrorMessage = "Código Entidade deve ter no máximo {1} caracteres")]
    public string CdentIdade { get; set; } = string.Empty;

    /// <summary>
    /// Data Base
    /// </summary>
    [Display(Name = "Data Base")]
    [StringLength(2, ErrorMessage = "Data Base deve ter no máximo {1} caracteres")]
    public string DataBase { get; set; } = string.Empty;

    /// <summary>
    /// Tipo
    /// </summary>
    [Display(Name = "Tipo")]
    public int? Fltipo { get; set; }

    /// <summary>
    /// Tabela Base
    /// </summary>
    [Display(Name = "Tabela Base")]
    [StringLength(3, ErrorMessage = "Tabela Base deve ter no máximo {1} caracteres")]
    public string Cdtabbase { get; set; } = string.Empty;
}
