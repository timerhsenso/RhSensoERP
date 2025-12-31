// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapBloqueiosPessoa
// Module: ControleAcessoPortaria
// Data: 2025-12-30 21:42:59
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapBloqueiosPessoa;

/// <summary>
/// Request para criacao de CapBloqueiosPessoa.
/// Compativel com backend: CreateCapBloqueiosPessoaRequest
/// </summary>
public class CreateCapBloqueiosPessoaRequest
{
    /// <summary>
    /// IdFuncionarioLegado
    /// </summary>
    [Display(Name = "IdFuncionarioLegado")]
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
    [Required(ErrorMessage = "Motivo e obrigatorio")]
    [StringLength(500, ErrorMessage = "Motivo deve ter no maximo {1} caracteres")]
    public string Motivo { get; set; } = string.Empty;

    /// <summary>
    /// DataBloqueio
    /// </summary>
    [Display(Name = "DataBloqueio")]
    [Required(ErrorMessage = "DataBloqueio e obrigatorio")]
    public DateTime DataBloqueio { get; set; }

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
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo e obrigatorio")]
    public bool Ativo { get; set; }
}
