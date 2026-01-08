// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: CapVisitantes
// Module: ControleAcessoPortaria
// Data: 2026-01-07 21:13:40
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapVisitantes;

/// <summary>
/// Request para criação de Cadastro de Visitantes.
/// Compatível com backend: CreateCapVisitantesRequest
/// </summary>
public class CreateCapVisitantesRequest
{
    /// <summary>
    /// Nome
    /// </summary>
    [Display(Name = "Nome")]
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(255, ErrorMessage = "Nome deve ter no máximo {1} caracteres")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// CPF
    /// </summary>
    [Display(Name = "CPF")]
    [StringLength(14, ErrorMessage = "CPF deve ter no máximo {1} caracteres")]
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// RG
    /// </summary>
    [Display(Name = "RG")]
    [StringLength(20, ErrorMessage = "RG deve ter no máximo {1} caracteres")]
    public string Rg { get; set; } = string.Empty;

    /// <summary>
    /// E-mail
    /// </summary>
    [Display(Name = "E-mail")]
    [StringLength(100, ErrorMessage = "E-mail deve ter no máximo {1} caracteres")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Telefone
    /// </summary>
    [Display(Name = "Telefone")]
    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo {1} caracteres")]
    public string Telefone { get; set; } = string.Empty;

    /// <summary>
    /// Empresa
    /// </summary>
    [Display(Name = "Empresa")]
    [StringLength(255, ErrorMessage = "Empresa deve ter no máximo {1} caracteres")]
    public string Empresa { get; set; } = string.Empty;

    /// <summary>
    /// ID Funcionário Responsável
    /// </summary>
    [Display(Name = "ID Funcionário Responsável")]
    public int? IdFuncionarioResponsavel { get; set; }

    /// <summary>
    /// Requer Responsável
    /// </summary>
    [Display(Name = "Requer Responsável")]
    [Required(ErrorMessage = "Requer Responsável é obrigatório")]
    public bool RequerResponsavel { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo é obrigatório")]
    public bool Ativo { get; set; }
}
