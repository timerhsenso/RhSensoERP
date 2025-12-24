// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: TreTiposTreinamento
// Module: TreinamentoDesenvolvimento
// Data: 2025-12-22 12:51:16
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.TreinamentoDesenvolvimento.TreTiposTreinamento;

/// <summary>
/// Request para atualização de Tipos de Treinamento.
/// Compatível com backend: UpdateTreTiposTreinamentoRequest
/// </summary>
public class UpdateTreTiposTreinamentoRequest
{
    /// <summary>
    /// Saas
    /// </summary>
    [Display(Name = "Saas")]
    [Required(ErrorMessage = "Saas é obrigatório")]
    public int IdSaas { get; set; }

    /// <summary>
    /// Nome
    /// </summary>
    [Display(Name = "Nome")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo {1} caracteres")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [StringLength(500, ErrorMessage = "Descrição deve ter no máximo {1} caracteres")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Codigo Nr
    /// </summary>
    [Display(Name = "Codigo Nr")]
    [StringLength(20, ErrorMessage = "Codigo Nr deve ter no máximo {1} caracteres")]
    public string CodigoNr { get; set; } = string.Empty;

    /// <summary>
    /// Dias Prazo Val Idade
    /// </summary>
    [Display(Name = "Dias Prazo Val Idade")]
    public int? DiasPrazoValIdade { get; set; }

    /// <summary>
    /// Obrigatorio
    /// </summary>
    [Display(Name = "Obrigatorio")]
    [Required(ErrorMessage = "Obrigatorio é obrigatório")]
    public bool Obrigatorio { get; set; }

    /// <summary>
    /// Aplicavel A
    /// </summary>
    [Display(Name = "Aplicavel A")]
    [StringLength(100, ErrorMessage = "Aplicavel A deve ter no máximo {1} caracteres")]
    public string AplicavelA { get; set; } = string.Empty;

    /// <summary>
    /// Carga Horaria
    /// </summary>
    [Display(Name = "Carga Horaria")]
    public int? CargaHoraria { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo é obrigatório")]
    public bool Ativo { get; set; }
}
