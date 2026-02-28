// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Gurh1
// Module: Seguranca
// Data: 2026-02-28 09:59:07
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Seguranca.Gurh1;

/// <summary>
/// Request para criação de Grupo de Usuários.
/// Compatível com backend: CreateGurh1Request
/// </summary>
public class CreateGurh1Request
{
    /// <summary>
    /// Código de Sistema
    /// (Chave Primária - obrigatório na criação)
    /// </summary>
    [Display(Name = "Código de Sistema")]
    [Required(ErrorMessage = "Código de Sistema é obrigatório")]
    [StringLength(10, ErrorMessage = "Código de Sistema deve ter no máximo {1} caracteres")]
    public string CdsiStema { get; set; } = string.Empty;

    /// <summary>
    /// Código de Gr User
    /// (Chave Primária - obrigatório na criação)
    /// </summary>
    [Display(Name = "Código de Gr User")]
    [Required(ErrorMessage = "Código de Gr User é obrigatório")]
    [StringLength(30, ErrorMessage = "Código de Gr User deve ter no máximo {1} caracteres")]
    public string Cdgruser { get; set; } = string.Empty;

    /// <summary>
    /// Descrição de Gr User
    /// </summary>
    [Display(Name = "Descrição de Gr User")]
    [StringLength(60, ErrorMessage = "Descrição de Gr User deve ter no máximo {1} caracteres")]
    public string Dcgruser { get; set; } = string.Empty;
}
