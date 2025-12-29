// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapVisitantes
// Module: ControleAcessoPortaria
// Data: 2025-12-28 19:08:36
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapVisitantes;

/// <summary>
/// Request para atualizacao de CapVisitantes.
/// Compativel com backend: UpdateCapVisitantesRequest
/// </summary>
public class UpdateCapVisitantesRequest
{
    /// <summary>
    /// Nome
    /// </summary>
    [Display(Name = "Nome")]
    [Required(ErrorMessage = "Nome e obrigatorio")]
    [StringLength(255, ErrorMessage = "Nome deve ter no maximo {1} caracteres")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// CPF
    /// </summary>
    [Display(Name = "CPF")]
    [StringLength(14, ErrorMessage = "CPF deve ter no maximo {1} caracteres")]
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// RG
    /// </summary>
    [Display(Name = "RG")]
    [StringLength(20, ErrorMessage = "RG deve ter no maximo {1} caracteres")]
    public string Rg { get; set; } = string.Empty;

    /// <summary>
    /// E-mail
    /// </summary>
    [Display(Name = "E-mail")]
    [StringLength(100, ErrorMessage = "E-mail deve ter no maximo {1} caracteres")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Telefone
    /// </summary>
    [Display(Name = "Telefone")]
    [StringLength(20, ErrorMessage = "Telefone deve ter no maximo {1} caracteres")]
    public string Telefone { get; set; } = string.Empty;

    /// <summary>
    /// Empresa
    /// </summary>
    [Display(Name = "Empresa")]
    [StringLength(255, ErrorMessage = "Empresa deve ter no maximo {1} caracteres")]
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
    [Required(ErrorMessage = "Requer Responsável e obrigatorio")]
    public bool RequerResponsavel { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo e obrigatorio")]
    public bool Ativo { get; set; }
}
