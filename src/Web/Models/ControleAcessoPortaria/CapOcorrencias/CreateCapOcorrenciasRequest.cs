// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapOcorrencias
// Module: ControleAcessoPortaria
// Data: 2025-12-30 21:29:17
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapOcorrencias;

/// <summary>
/// Request para criacao de CapOcorrencias.
/// Compativel com backend: CreateCapOcorrenciasRequest
/// </summary>
public class CreateCapOcorrenciasRequest
{
    /// <summary>
    /// IdTipoOcorrencia
    /// </summary>
    [Display(Name = "IdTipoOcorrencia")]
    [Required(ErrorMessage = "IdTipoOcorrencia e obrigatorio")]
    public int IdTipoOcorrencia { get; set; }

    /// <summary>
    /// IdAcessoPortaria
    /// </summary>
    [Display(Name = "IdAcessoPortaria")]
    public int? IdAcessoPortaria { get; set; }

    /// <summary>
    /// IdFuncionarioResponsavel
    /// </summary>
    [Display(Name = "IdFuncionarioResponsavel")]
    public int? IdFuncionarioResponsavel { get; set; }

    /// <summary>
    /// IdColaboradorResponsavel
    /// </summary>
    [Display(Name = "IdColaboradorResponsavel")]
    public int? IdColaboradorResponsavel { get; set; }

    /// <summary>
    /// Descricao
    /// </summary>
    [Display(Name = "Descricao")]
    [Required(ErrorMessage = "Descricao e obrigatorio")]
    [StringLength(500, ErrorMessage = "Descricao deve ter no maximo {1} caracteres")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// DataOcorrencia
    /// </summary>
    [Display(Name = "DataOcorrencia")]
    [Required(ErrorMessage = "DataOcorrencia e obrigatorio")]
    public DateTime DataOcorrencia { get; set; }

    /// <summary>
    /// Local
    /// </summary>
    [Display(Name = "Local")]
    [StringLength(500, ErrorMessage = "Local deve ter no maximo {1} caracteres")]
    public string Local { get; set; } = string.Empty;

    /// <summary>
    /// Observacoes
    /// </summary>
    [Display(Name = "Observacoes")]
    [StringLength(500, ErrorMessage = "Observacoes deve ter no maximo {1} caracteres")]
    public string Observacoes { get; set; } = string.Empty;

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
