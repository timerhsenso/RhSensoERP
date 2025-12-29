// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: TreTiposTreinamento
// Module: GestaoDePessoas
// Data: 2025-12-28 14:07:48
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.GestaoDePessoas.TreTiposTreinamento;

/// <summary>
/// Request para atualizacao de Tipos de Treinamento.
/// Compativel com backend: UpdateTreTiposTreinamentoRequest
/// </summary>
public class UpdateTreTiposTreinamentoRequest
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
    /// DiasPrazoValidade
    /// </summary>
    [Display(Name = "DiasPrazoValidade")]
    public int? DiasPrazoValidade { get; set; }

    /// <summary>
    /// Obrigatório
    /// </summary>
    [Display(Name = "Obrigatório")]
    [Required(ErrorMessage = "Obrigatório e obrigatorio")]
    public bool Obrigatorio { get; set; }

    /// <summary>
    /// AplicavelA
    /// </summary>
    [Display(Name = "AplicavelA")]
    [StringLength(100, ErrorMessage = "AplicavelA deve ter no maximo {1} caracteres")]
    public string AplicavelA { get; set; } = string.Empty;

    /// <summary>
    /// CargaHoraria
    /// </summary>
    [Display(Name = "CargaHoraria")]
    public int? CargaHoraria { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo e obrigatorio")]
    public bool Ativo { get; set; }
}
