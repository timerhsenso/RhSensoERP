// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: CapTiposOcorrencia
// Module: ControleAcessoPortaria
// Data: 2026-01-07 21:19:53
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapTiposOcorrencia;

/// <summary>
/// Request para atualização de Cadastro de Tipo de Ocorrência.
/// Compatível com backend: UpdateCapTiposOcorrenciaRequest
/// </summary>
public class UpdateCapTiposOcorrenciaRequest
{
    /// <summary>
    /// Nome
    /// </summary>
    [Display(Name = "Nome")]
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo {1} caracteres")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [StringLength(500, ErrorMessage = "Descrição deve ter no máximo {1} caracteres")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Severidade
    /// </summary>
    [Display(Name = "Severidade")]
    [StringLength(50, ErrorMessage = "Severidade deve ter no máximo {1} caracteres")]
    public string Severidade { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo é obrigatório")]
    public bool Ativo { get; set; }
}
