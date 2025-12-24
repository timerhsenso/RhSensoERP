// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: BasFeriados
// Module: GestaoDePessoas
// Data: 2025-12-22 23:31:27
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.GestaoDePessoas.BasFeriados;

/// <summary>
/// Request para atualização de BasFeriados.
/// Compatível com backend: UpdateBasFeriadosRequest
/// </summary>
public class UpdateBasFeriadosRequest
{
    /// <summary>
    /// Tenant Id
    /// </summary>
    [Display(Name = "Tenant Id")]
    [Required(ErrorMessage = "Tenant Id é obrigatório")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Data
    /// </summary>
    [Display(Name = "Data")]
    [Required(ErrorMessage = "Data é obrigatório")]
    public DateOnly Data { get; set; }

    /// <summary>
    /// Descricao
    /// </summary>
    [Display(Name = "Descricao")]
    [StringLength(255, ErrorMessage = "Descricao deve ter no máximo {1} caracteres")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Tipo
    /// </summary>
    [Display(Name = "Tipo")]
    [StringLength(50, ErrorMessage = "Tipo deve ter no máximo {1} caracteres")]
    public string Tipo { get; set; } = string.Empty;

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
