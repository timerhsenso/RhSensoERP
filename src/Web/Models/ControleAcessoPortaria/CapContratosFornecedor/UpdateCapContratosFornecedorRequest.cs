// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapContratosFornecedor
// Module: ControleAcessoPortaria
// Data: 2025-12-30 21:36:02
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapContratosFornecedor;

/// <summary>
/// Request para atualizacao de CapContratosFornecedor.
/// Compativel com backend: UpdateCapContratosFornecedorRequest
/// </summary>
public class UpdateCapContratosFornecedorRequest
{
    /// <summary>
    /// Fornecedor
    /// </summary>
    [Display(Name = "Fornecedor")]
    [Required(ErrorMessage = "Fornecedor e obrigatorio")]
    public int IdFornecedor { get; set; }

    /// <summary>
    /// NumeroContrato
    /// </summary>
    [Display(Name = "NumeroContrato")]
    [Required(ErrorMessage = "NumeroContrato e obrigatorio")]
    [StringLength(100, ErrorMessage = "NumeroContrato deve ter no maximo {1} caracteres")]
    public string NumeroContrato { get; set; } = string.Empty;

    /// <summary>
    /// DataInicio
    /// </summary>
    [Display(Name = "DataInicio")]
    [Required(ErrorMessage = "DataInicio e obrigatorio")]
    public DateOnly DataInicio { get; set; }

    /// <summary>
    /// Data Fim
    /// </summary>
    [Display(Name = "Data Fim")]
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
    [StringLength(500, ErrorMessage = "Descrição deve ter no maximo {1} caracteres")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Status
    /// </summary>
    [Display(Name = "Status")]
    [StringLength(50, ErrorMessage = "Status deve ter no maximo {1} caracteres")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo e obrigatorio")]
    public bool Ativo { get; set; }
}
