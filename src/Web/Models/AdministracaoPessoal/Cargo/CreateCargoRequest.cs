// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Cargo
// Module: AdministracaoPessoal
// Data: 2026-02-28 22:19:49
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.Cargo;

/// <summary>
/// Request para criação de Cargos.
/// Compatível com backend: CreateCargoRequest
/// </summary>
public class CreateCargoRequest
{
    /// <summary>
    /// Código
    /// </summary>
    [Display(Name = "Código")]
    [StringLength(5, ErrorMessage = "Código deve ter no máximo {1} caracteres")]
    public string Cdcargo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [StringLength(50, ErrorMessage = "Descrição deve ter no máximo {1} caracteres")]
    public string Dccargo { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo é obrigatório")]
    public int Flativo { get; set; }

    /// <summary>
    /// Data Início Validade
    /// </summary>
    [Display(Name = "Data Início Validade")]
    public DateTime? Dtinival { get; set; }

    /// <summary>
    /// Data Fim Validade
    /// </summary>
    [Display(Name = "Data Fim Validade")]
    public DateTime? Dtfimval { get; set; }

    /// <summary>
    /// Código Tabela Salarial
    /// </summary>
    [Display(Name = "Código Tabela Salarial")]
    [StringLength(3, ErrorMessage = "Código Tabela Salarial deve ter no máximo {1} caracteres")]
    public string Cdtabela { get; set; } = string.Empty;

    /// <summary>
    /// Nível Inicial
    /// </summary>
    [Display(Name = "Nível Inicial")]
    [StringLength(5, ErrorMessage = "Nível Inicial deve ter no máximo {1} caracteres")]
    public string Cdniveini { get; set; } = string.Empty;

    /// <summary>
    /// Nível Final
    /// </summary>
    [Display(Name = "Nível Final")]
    [StringLength(5, ErrorMessage = "Nível Final deve ter no máximo {1} caracteres")]
    public string Cdnivefim { get; set; } = string.Empty;

    /// <summary>
    /// Código Instrução
    /// </summary>
    [Display(Name = "Código Instrução")]
    [StringLength(2, ErrorMessage = "Código Instrução deve ter no máximo {1} caracteres")]
    public string CdinStruc { get; set; } = string.Empty;

    /// <summary>
    /// Código CBO (5 dígitos)
    /// </summary>
    [Display(Name = "Código CBO (5 dígitos)")]
    [StringLength(5, ErrorMessage = "Código CBO (5 dígitos) deve ter no máximo {1} caracteres")]
    public string Cdcbo { get; set; } = string.Empty;

    /// <summary>
    /// Código CBO (6 dígitos)
    /// </summary>
    [Display(Name = "Código CBO (6 dígitos)")]
    [StringLength(6, ErrorMessage = "Código CBO (6 dígitos) deve ter no máximo {1} caracteres")]
    public string Cdcbo6 { get; set; } = string.Empty;

    /// <summary>
    /// Grupo Profissional
    /// </summary>
    [Display(Name = "Grupo Profissional")]
    [StringLength(2, ErrorMessage = "Grupo Profissional deve ter no máximo {1} caracteres")]
    public string Cdgrprof { get; set; } = string.Empty;

    /// <summary>
    /// Tenant
    /// </summary>
    [Display(Name = "Tenant")]
    [Required(ErrorMessage = "Tenant é obrigatório")]
    public int Tenant { get; set; }

    /// <summary>
    /// Idcbo
    /// </summary>
    [Display(Name = "Idcbo")]
    public Guid? Idcbo { get; set; }

    /// <summary>
    /// Idgraudeinstrucao
    /// </summary>
    [Display(Name = "Idgraudeinstrucao")]
    public Guid? IdgraudeinStrucao { get; set; }

    /// <summary>
    /// Idtabelasalarial
    /// </summary>
    [Display(Name = "Idtabelasalarial")]
    public Guid? Idtabelasalarial { get; set; }
}
