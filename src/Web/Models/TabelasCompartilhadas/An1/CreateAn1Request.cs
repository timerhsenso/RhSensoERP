// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: An1
// Module: TabelasCompartilhadas
// Data: 2026-02-25 16:30:15
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.TabelasCompartilhadas.An1;

/// <summary>
/// Request para criação de Tabela de Bancos.
/// Compatível com backend: CreateAn1Request
/// </summary>
public class CreateAn1Request
{
    /// <summary>
    /// Código
    /// </summary>
    [Display(Name = "Código")]
    [StringLength(3, ErrorMessage = "Código deve ter no máximo {1} caracteres")]
    public string Cdbanco { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [StringLength(40, ErrorMessage = "Descrição deve ter no máximo {1} caracteres")]
    public string Dcbanco { get; set; } = string.Empty;
}
