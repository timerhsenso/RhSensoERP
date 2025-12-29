// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapOcorrencias
// Module: ControleAcessoPortaria
// Data: 2025-12-28 20:36:06
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapOcorrencias;

/// <summary>
/// Request para atualizacao de CapOcorrencias.
/// Compativel com backend: UpdateCapOcorrenciasRequest
/// </summary>
public class UpdateCapOcorrenciasRequest
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
    /// ID Funcionário Responsável
    /// </summary>
    [Display(Name = "ID Funcionário Responsável")]
    public int? IdFuncionarioResponsavel { get; set; }

    /// <summary>
    /// ID Colaborador Responsável
    /// </summary>
    [Display(Name = "ID Colaborador Responsável")]
    public int? IdColaboradorResponsavel { get; set; }

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [Required(ErrorMessage = "Descrição e obrigatorio")]
    [StringLength(500, ErrorMessage = "Descrição deve ter no maximo {1} caracteres")]
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
    /// Observações
    /// </summary>
    [Display(Name = "Observações")]
    [StringLength(500, ErrorMessage = "Observações deve ter no maximo {1} caracteres")]
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
