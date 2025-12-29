// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapTiposOcorrencia
// Module: ControleAcessoPortaria
// Data: 2025-12-28 19:27:50
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapTiposOcorrencia;

/// <summary>
/// Request para atualizacao de CapTiposOcorrencia.
/// Compativel com backend: UpdateCapTiposOcorrenciaRequest
/// </summary>
public class UpdateCapTiposOcorrenciaRequest
{
    /// <summary>
    /// Nome
    /// </summary>
    [Display(Name = "Nome")]
    [Required(ErrorMessage = "Nome e obrigatorio")]
    [StringLength(100, ErrorMessage = "Nome deve ter no maximo {1} caracteres")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [StringLength(500, ErrorMessage = "Descrição deve ter no maximo {1} caracteres")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Severidade
    /// </summary>
    [Display(Name = "Severidade")]
    [StringLength(50, ErrorMessage = "Severidade deve ter no maximo {1} caracteres")]
    public string Severidade { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo e obrigatorio")]
    public bool Ativo { get; set; }
}
