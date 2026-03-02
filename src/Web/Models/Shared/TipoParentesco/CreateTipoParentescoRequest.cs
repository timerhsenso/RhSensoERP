// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TipoParentesco
// Module: Shared
// Data: 2026-03-02 17:59:26
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Shared.TipoParentesco;

/// <summary>
/// Request para criação de Tipo de Parentesco.
/// Compatível com backend: CreateTipoParentescoRequest
/// </summary>
public class CreateTipoParentescoRequest
{
    /// <summary>
    /// Código
    /// </summary>
    [Display(Name = "Código")]
    [Required(ErrorMessage = "Código é obrigatório")]
    [StringLength(20, ErrorMessage = "Código deve ter no máximo {1} caracteres")]
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [Required(ErrorMessage = "Descrição é obrigatório")]
    [StringLength(50, ErrorMessage = "Descrição deve ter no máximo {1} caracteres")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Ordem
    /// </summary>
    [Display(Name = "Ordem")]
    [Required(ErrorMessage = "Ordem é obrigatório")]
    public byte Ordem { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo é obrigatório")]
    public bool Ativo { get; set; }
}
