// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: HabilitacaoBotao
// Module: Seguranca
// Data: 2026-03-01 13:35:46
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Seguranca.HabilitacaoBotao;

/// <summary>
/// Request para criação de Hab botao.
/// Compatível com backend: CreateHabilitacaoBotaoRequest
/// </summary>
public class CreateHabilitacaoBotaoRequest
{
    /// <summary>
    /// Código de Gr User
    /// </summary>
    [Display(Name = "Código de Gr User")]
    [Required(ErrorMessage = "Código de Gr User é obrigatório")]
    [StringLength(30, ErrorMessage = "Código de Gr User deve ter no máximo {1} caracteres")]
    public string Cdgruser { get; set; } = string.Empty;

    /// <summary>
    /// Código de Funcao
    /// </summary>
    [Display(Name = "Código de Funcao")]
    [Required(ErrorMessage = "Código de Funcao é obrigatório")]
    [StringLength(30, ErrorMessage = "Código de Funcao deve ter no máximo {1} caracteres")]
    public string Cdfuncao { get; set; } = string.Empty;

    /// <summary>
    /// Código de Sistema
    /// </summary>
    [Display(Name = "Código de Sistema")]
    [Required(ErrorMessage = "Código de Sistema é obrigatório")]
    [StringLength(10, ErrorMessage = "Código de Sistema deve ter no máximo {1} caracteres")]
    public string CdsiStema { get; set; } = string.Empty;

    /// <summary>
    /// Nome de Botao
    /// </summary>
    [Display(Name = "Nome de Botao")]
    [Required(ErrorMessage = "Nome de Botao é obrigatório")]
    [StringLength(30, ErrorMessage = "Nome de Botao deve ter no máximo {1} caracteres")]
    public string Nmbotao { get; set; } = string.Empty;

    /// <summary>
    /// Idgrupodeusuariofuncao
    /// </summary>
    [Display(Name = "Idgrupodeusuariofuncao")]
    public Guid? Idgrupodeusuariofuncao { get; set; }
}
