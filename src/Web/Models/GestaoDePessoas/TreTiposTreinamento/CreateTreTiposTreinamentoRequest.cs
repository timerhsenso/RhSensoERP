// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: TreTiposTreinamento
// Module: GestaoDePessoas
// Data: 2025-12-30 05:27:05
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.GestaoDePessoas.TreTiposTreinamento;

/// <summary>
/// Request para criacao de Tipos de Treinamento.
/// Compativel com backend: CreateTreTiposTreinamentoRequest
/// </summary>
public class CreateTreTiposTreinamentoRequest
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
    /// CodigoNr
    /// </summary>
    [Display(Name = "CodigoNr")]
    [StringLength(20, ErrorMessage = "CodigoNr deve ter no maximo {1} caracteres")]
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
    [Required(ErrorMessage = "Obrigatório e obrigatorio")]
    public bool Obrigatorio { get; set; }

    /// <summary>
    /// Aplicável a
    /// </summary>
    [Display(Name = "Aplicável a")]
    [StringLength(100, ErrorMessage = "Aplicável a deve ter no maximo {1} caracteres")]
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
    [Required(ErrorMessage = "Ativo e obrigatorio")]
    public bool Ativo { get; set; }
}
