// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Fucn1
// Module: Seguranca
// Data: 2026-02-28 01:25:19
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Seguranca.Fucn1;

/// <summary>
/// Request para atualização de Cadastro de Terceiros.
/// Compatível com backend: UpdateFucn1Request
/// </summary>
public class UpdateFucn1Request
{
    /// <summary>
    /// Descrição de Funcao
    /// </summary>
    [Display(Name = "Descrição de Funcao")]
    [StringLength(80, ErrorMessage = "Descrição de Funcao deve ter no máximo {1} caracteres")]
    public string Dcfuncao { get; set; } = string.Empty;

    /// <summary>
    /// Descrição de Modulo
    /// </summary>
    [Display(Name = "Descrição de Modulo")]
    [StringLength(100, ErrorMessage = "Descrição de Modulo deve ter no máximo {1} caracteres")]
    public string Dcmodulo { get; set; } = string.Empty;

    /// <summary>
    /// Descricao Modulo
    /// </summary>
    [Display(Name = "Descricao Modulo")]
    [StringLength(100, ErrorMessage = "Descricao Modulo deve ter no máximo {1} caracteres")]
    public string Descricaomodulo { get; set; } = string.Empty;
}
