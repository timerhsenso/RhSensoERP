// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: CapVisitantes
// Module: ControleAcessoPortaria
// Data: 2025-12-24 01:10:14
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapVisitantes;

/// <summary>
/// Request para atualização de CapVisitantes.
/// Compatível com backend: UpdateCapVisitantesRequest
/// </summary>
public class UpdateCapVisitantesRequest
{
    /// <summary>
    /// Tenant Id
    /// </summary>
    [Display(Name = "Tenant Id")]
    [Required(ErrorMessage = "Tenant Id é obrigatório")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Nome
    /// </summary>
    [Display(Name = "Nome")]
    [StringLength(255, ErrorMessage = "Nome deve ter no máximo {1} caracteres")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Cpf
    /// </summary>
    [Display(Name = "Cpf")]
    [StringLength(14, ErrorMessage = "Cpf deve ter no máximo {1} caracteres")]
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// RG
    /// </summary>
    [Display(Name = "RG")]
    [StringLength(20, ErrorMessage = "RG deve ter no máximo {1} caracteres")]
    public string Rg { get; set; } = string.Empty;

    /// <summary>
    /// Email
    /// </summary>
    [Display(Name = "Email")]
    [StringLength(100, ErrorMessage = "Email deve ter no máximo {1} caracteres")]
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

    /// <summary>
    /// Created At Utc
    /// </summary>
    [Display(Name = "Created At Utc")]
    [Required(ErrorMessage = "Created At Utc é obrigatório")]
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Created By User Id
    /// </summary>
    [Display(Name = "Created By User Id")]
    public Guid? CreatedByUserId { get; set; }

    /// <summary>
    /// Updated At Utc
    /// </summary>
    [Display(Name = "Updated At Utc")]
    [Required(ErrorMessage = "Updated At Utc é obrigatório")]
    public DateTime UpdatedAtUtc { get; set; }

    /// <summary>
    /// Updated By User Id
    /// </summary>
    [Display(Name = "Updated By User Id")]
    public Guid? UpdatedByUserId { get; set; }
}
