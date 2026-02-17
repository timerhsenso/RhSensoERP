// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TreTiposTreinamento
// Module: TreinamentoDesenvolvimento
// Data: 2026-02-16 23:23:32
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.TreinamentoDesenvolvimento.TreTiposTreinamento;

/// <summary>
/// Request para criação de Cadastro de Tipo de Treinamento.
/// Compatível com backend: CreateTreTiposTreinamentoRequest
/// </summary>
public class CreateTreTiposTreinamentoRequest
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
    /// Código NR
    /// </summary>
    [Display(Name = "Código NR")]
    [StringLength(20, ErrorMessage = "Código NR deve ter no máximo {1} caracteres")]
    public string CodigoNr { get; set; } = string.Empty;

    /// <summary>
    /// Validade (dias)
    /// </summary>
    [Display(Name = "Validade (dias)")]
    public int? DiasPrazoValidade { get; set; }

    /// <summary>
    /// Obrigatório
    /// </summary>
    [Display(Name = "Obrigatório")]
    [Required(ErrorMessage = "Obrigatório é obrigatório")]
    public bool Obrigatorio { get; set; }

    /// <summary>
    /// Aplicável a
    /// </summary>
    [Display(Name = "Aplicável a")]
    [StringLength(100, ErrorMessage = "Aplicável a deve ter no máximo {1} caracteres")]
    public string AplicavelA { get; set; } = string.Empty;

    /// <summary>
    /// Carga horária (h)
    /// </summary>
    [Display(Name = "Carga horária (h)")]
    public int? CargaHoraria { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo é obrigatório")]
    public bool Ativo { get; set; }
}
