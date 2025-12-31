// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapHistoricoBloqueios
// Module: ControleAcessoPortaria
// Data: 2025-12-30 21:31:32
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapHistoricoBloqueios;

/// <summary>
/// Request para criacao de CapHistoricoBloqueios.
/// Compativel com backend: CreateCapHistoricoBloqueiosRequest
/// </summary>
public class CreateCapHistoricoBloqueiosRequest
{
    /// <summary>
    /// Bloqueio
    /// </summary>
    [Display(Name = "Bloqueio")]
    [Required(ErrorMessage = "Bloqueio e obrigatorio")]
    public int IdBloqueio { get; set; }

    /// <summary>
    /// ID Funcionário Legado
    /// </summary>
    [Display(Name = "ID Funcionário Legado")]
    public int? IdFuncionarioLegado { get; set; }

    /// <summary>
    /// ID Colaborador Fornecedor
    /// </summary>
    [Display(Name = "ID Colaborador Fornecedor")]
    public int? IdColaboradorFornecedor { get; set; }

    /// <summary>
    /// IdVisitante
    /// </summary>
    [Display(Name = "IdVisitante")]
    public int? IdVisitante { get; set; }

    /// <summary>
    /// Motivo
    /// </summary>
    [Display(Name = "Motivo")]
    [StringLength(500, ErrorMessage = "Motivo deve ter no maximo {1} caracteres")]
    public string Motivo { get; set; } = string.Empty;

    /// <summary>
    /// DataBloqueio
    /// </summary>
    [Display(Name = "DataBloqueio")]
    public DateTime? DataBloqueio { get; set; }

    /// <summary>
    /// Data Desbloqueio (UTC)
    /// </summary>
    [Display(Name = "Data Desbloqueio (UTC)")]
    public DateTime? DataDesbloqueio { get; set; }

    /// <summary>
    /// Usuário Bloqueio
    /// </summary>
    [Display(Name = "Usuário Bloqueio")]
    public Guid? UsuarioBloqueio { get; set; }

    /// <summary>
    /// Usuário Desbloqueio
    /// </summary>
    [Display(Name = "Usuário Desbloqueio")]
    public Guid? UsuarioDesbloqueio { get; set; }

    /// <summary>
    /// Data Registro
    /// </summary>
    [Display(Name = "Data Registro")]
    [Required(ErrorMessage = "Data Registro e obrigatorio")]
    public DateTime DataRegistro { get; set; }
}
