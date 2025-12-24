// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: CapResponsaveisContrato
// Module: ControleAcessoPortaria
// Data: 2025-12-24 01:15:50
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapResponsaveisContrato;

/// <summary>
/// Request para atualização de CapResponsaveisContrato.
/// Compatível com backend: UpdateCapResponsaveisContratoRequest
/// </summary>
public class UpdateCapResponsaveisContratoRequest
{
    /// <summary>
    /// Tenant ID
    /// </summary>
    [Display(Name = "Tenant ID")]
    [Required(ErrorMessage = "Tenant ID é obrigatório")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// IdContrato
    /// </summary>
    [Display(Name = "IdContrato")]
    [Required(ErrorMessage = "IdContrato é obrigatório")]
    public int IdContrato { get; set; }

    /// <summary>
    /// IdFuncionarioLegado
    /// </summary>
    [Display(Name = "IdFuncionarioLegado")]
    public int? IdFuncionarioLegado { get; set; }

    /// <summary>
    /// TipoResponsabilidade
    /// </summary>
    [Display(Name = "TipoResponsabilidade")]
    [StringLength(100, ErrorMessage = "TipoResponsabilidade deve ter no máximo {1} caracteres")]
    public string TipoResponsabilidade { get; set; } = string.Empty;

    /// <summary>
    /// DataInicio
    /// </summary>
    [Display(Name = "DataInicio")]
    public DateOnly? DataInicio { get; set; }

    /// <summary>
    /// DataFim
    /// </summary>
    [Display(Name = "DataFim")]
    public DateOnly? DataFim { get; set; }

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
