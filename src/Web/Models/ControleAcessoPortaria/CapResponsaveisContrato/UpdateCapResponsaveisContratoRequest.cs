// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapResponsaveisContrato
// Module: ControleAcessoPortaria
// Data: 2025-12-28 20:29:17
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapResponsaveisContrato;

/// <summary>
/// Request para atualizacao de CapResponsaveisContrato.
/// Compativel com backend: UpdateCapResponsaveisContratoRequest
/// </summary>
public class UpdateCapResponsaveisContratoRequest
{
    /// <summary>
    /// IdContrato
    /// </summary>
    [Display(Name = "IdContrato")]
    [Required(ErrorMessage = "IdContrato e obrigatorio")]
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
    [StringLength(100, ErrorMessage = "TipoResponsabilidade deve ter no maximo {1} caracteres")]
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
    [Required(ErrorMessage = "Ativo e obrigatorio")]
    public bool Ativo { get; set; }
}
