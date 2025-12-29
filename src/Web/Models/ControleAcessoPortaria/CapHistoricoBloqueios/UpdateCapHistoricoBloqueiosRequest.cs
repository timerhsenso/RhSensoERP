// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapHistoricoBloqueios
// Module: ControleAcessoPortaria
// Data: 2025-12-28 20:51:29
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapHistoricoBloqueios;

/// <summary>
/// Request para atualizacao de CapHistoricoBloqueios.
/// Compativel com backend: UpdateCapHistoricoBloqueiosRequest
/// </summary>
public class UpdateCapHistoricoBloqueiosRequest
{
    /// <summary>
    /// IdBloqueio
    /// </summary>
    [Display(Name = "IdBloqueio")]
    [Required(ErrorMessage = "IdBloqueio e obrigatorio")]
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
    /// ID Visitante
    /// </summary>
    [Display(Name = "ID Visitante")]
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
    /// Data Registro (UTC)
    /// </summary>
    [Display(Name = "Data Registro (UTC)")]
    [Required(ErrorMessage = "Data Registro (UTC) e obrigatorio")]
    public DateTime DataRegistro { get; set; }
}
