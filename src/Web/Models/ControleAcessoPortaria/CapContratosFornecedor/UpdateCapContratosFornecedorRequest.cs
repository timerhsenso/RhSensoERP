// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: CapContratosFornecedor
// Module: ControleAcessoPortaria
// Data: 2025-12-24 01:21:44
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapContratosFornecedor;

/// <summary>
/// Request para atualização de CapContratosFornecedor.
/// Compatível com backend: UpdateCapContratosFornecedorRequest
/// </summary>
public class UpdateCapContratosFornecedorRequest
{
    /// <summary>
    /// Tenant ID
    /// </summary>
    [Display(Name = "Tenant ID")]
    [Required(ErrorMessage = "Tenant ID é obrigatório")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// IdFornecedor
    /// </summary>
    [Display(Name = "IdFornecedor")]
    [Required(ErrorMessage = "IdFornecedor é obrigatório")]
    public int IdFornecedor { get; set; }

    /// <summary>
    /// NumeroContrato
    /// </summary>
    [Display(Name = "NumeroContrato")]
    [StringLength(100, ErrorMessage = "NumeroContrato deve ter no máximo {1} caracteres")]
    public string NumeroContrato { get; set; } = string.Empty;

    /// <summary>
    /// DataInicio
    /// </summary>
    [Display(Name = "DataInicio")]
    [Required(ErrorMessage = "DataInicio é obrigatório")]
    public DateOnly DataInicio { get; set; }

    /// <summary>
    /// DataFim
    /// </summary>
    [Display(Name = "DataFim")]
    public DateOnly? DataFim { get; set; }

    /// <summary>
    /// Valor
    /// </summary>
    [Display(Name = "Valor")]
    public decimal? Valor { get; set; }

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [StringLength(500, ErrorMessage = "Descrição deve ter no máximo {1} caracteres")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Status
    /// </summary>
    [Display(Name = "Status")]
    [StringLength(50, ErrorMessage = "Status deve ter no máximo {1} caracteres")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo é obrigatório")]
    public bool Ativo { get; set; }

    /// <summary>
    /// Data Criação (UTC)
    /// </summary>
    [Display(Name = "Data Criação (UTC)")]
    [Required(ErrorMessage = "Data Criação (UTC) é obrigatório")]
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Criado Por
    /// </summary>
    [Display(Name = "Criado Por")]
    public Guid? CreatedByUserId { get; set; }

    /// <summary>
    /// Data Atualização (UTC)
    /// </summary>
    [Display(Name = "Data Atualização (UTC)")]
    [Required(ErrorMessage = "Data Atualização (UTC) é obrigatório")]
    public DateTime UpdatedAtUtc { get; set; }

    /// <summary>
    /// Atualizado Por
    /// </summary>
    [Display(Name = "Atualizado Por")]
    public Guid? UpdatedByUserId { get; set; }
}
