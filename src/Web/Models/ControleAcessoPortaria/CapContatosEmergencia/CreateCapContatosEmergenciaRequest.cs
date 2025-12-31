// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapContatosEmergencia
// Module: ControleAcessoPortaria
// Data: 2025-12-30 21:38:31
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapContatosEmergencia;

/// <summary>
/// Request para criacao de CapContatosEmergencia.
/// Compativel com backend: CreateCapContatosEmergenciaRequest
/// </summary>
public class CreateCapContatosEmergenciaRequest
{
    /// <summary>
    /// ID Funcionário Legado
    /// </summary>
    [Display(Name = "ID Funcionário Legado")]
    public int? IdFuncionarioLegado { get; set; }

    /// <summary>
    /// IdColaboradorFornecedor
    /// </summary>
    [Display(Name = "IdColaboradorFornecedor")]
    public int? IdColaboradorFornecedor { get; set; }

    /// <summary>
    /// Nome
    /// </summary>
    [Display(Name = "Nome")]
    [Required(ErrorMessage = "Nome e obrigatorio")]
    [StringLength(255, ErrorMessage = "Nome deve ter no maximo {1} caracteres")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// IdParentesco
    /// </summary>
    [Display(Name = "IdParentesco")]
    public int? IdParentesco { get; set; }

    /// <summary>
    /// TelefonePrincipal
    /// </summary>
    [Display(Name = "TelefonePrincipal")]
    [Required(ErrorMessage = "TelefonePrincipal e obrigatorio")]
    [StringLength(20, ErrorMessage = "TelefonePrincipal deve ter no maximo {1} caracteres")]
    public string TelefonePrincipal { get; set; } = string.Empty;

    /// <summary>
    /// Telefone Secundário
    /// </summary>
    [Display(Name = "Telefone Secundário")]
    [StringLength(20, ErrorMessage = "Telefone Secundário deve ter no maximo {1} caracteres")]
    public string TelefoneSecundario { get; set; } = string.Empty;

    /// <summary>
    /// Email
    /// </summary>
    [Display(Name = "Email")]
    [StringLength(100, ErrorMessage = "Email deve ter no maximo {1} caracteres")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Endereço
    /// </summary>
    [Display(Name = "Endereço")]
    [StringLength(500, ErrorMessage = "Endereço deve ter no maximo {1} caracteres")]
    public string Endereco { get; set; } = string.Empty;

    /// <summary>
    /// Cidade
    /// </summary>
    [Display(Name = "Cidade")]
    [StringLength(100, ErrorMessage = "Cidade deve ter no maximo {1} caracteres")]
    public string Cidade { get; set; } = string.Empty;

    /// <summary>
    /// ID UF
    /// </summary>
    [Display(Name = "ID UF")]
    public int? IdUf { get; set; }

    /// <summary>
    /// CEP
    /// </summary>
    [Display(Name = "CEP")]
    [StringLength(10, ErrorMessage = "CEP deve ter no maximo {1} caracteres")]
    public string Cep { get; set; } = string.Empty;

    /// <summary>
    /// Ordem Prioridade
    /// </summary>
    [Display(Name = "Ordem Prioridade")]
    [Required(ErrorMessage = "Ordem Prioridade e obrigatorio")]
    public int OrdemPrioridade { get; set; }

    /// <summary>
    /// Observações
    /// </summary>
    [Display(Name = "Observações")]
    [StringLength(500, ErrorMessage = "Observações deve ter no maximo {1} caracteres")]
    public string Observacoes { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo e obrigatorio")]
    public bool Ativo { get; set; }
}
