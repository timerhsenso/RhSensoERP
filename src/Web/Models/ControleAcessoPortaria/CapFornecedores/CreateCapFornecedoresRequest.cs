// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapFornecedores
// Module: ControleAcessoPortaria
// Data: 2025-12-28 17:46:18
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapFornecedores;

/// <summary>
/// Request para criacao de CapFornecedores.
/// Compativel com backend: CreateCapFornecedoresRequest
/// </summary>
public class CreateCapFornecedoresRequest
{
    /// <summary>
    /// RazaoSocial
    /// </summary>
    [Display(Name = "RazaoSocial")]
    [Required(ErrorMessage = "RazaoSocial e obrigatorio")]
    [StringLength(255, ErrorMessage = "RazaoSocial deve ter no maximo {1} caracteres")]
    public string RazaoSocial { get; set; } = string.Empty;

    /// <summary>
    /// Nome Fantasia
    /// </summary>
    [Display(Name = "Nome Fantasia")]
    [StringLength(255, ErrorMessage = "Nome Fantasia deve ter no maximo {1} caracteres")]
    public string NomeFantasia { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ
    /// </summary>
    [Display(Name = "CNPJ")]
    [StringLength(18, ErrorMessage = "CNPJ deve ter no maximo {1} caracteres")]
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>
    /// CPF
    /// </summary>
    [Display(Name = "CPF")]
    [StringLength(14, ErrorMessage = "CPF deve ter no maximo {1} caracteres")]
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// Email
    /// </summary>
    [Display(Name = "Email")]
    [StringLength(100, ErrorMessage = "Email deve ter no maximo {1} caracteres")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Telefone
    /// </summary>
    [Display(Name = "Telefone")]
    [StringLength(20, ErrorMessage = "Telefone deve ter no maximo {1} caracteres")]
    public string Telefone { get; set; } = string.Empty;

    /// <summary>
    /// Endereço
    /// </summary>
    [Display(Name = "Endereço")]
    [StringLength(500, ErrorMessage = "Endereço deve ter no maximo {1} caracteres")]
    public string Endereco { get; set; } = string.Empty;

    /// <summary>
    /// Número
    /// </summary>
    [Display(Name = "Número")]
    [StringLength(20, ErrorMessage = "Número deve ter no maximo {1} caracteres")]
    public string Numero { get; set; } = string.Empty;

    /// <summary>
    /// Complemento
    /// </summary>
    [Display(Name = "Complemento")]
    [StringLength(200, ErrorMessage = "Complemento deve ter no maximo {1} caracteres")]
    public string Complemento { get; set; } = string.Empty;

    /// <summary>
    /// Bairro
    /// </summary>
    [Display(Name = "Bairro")]
    [StringLength(100, ErrorMessage = "Bairro deve ter no maximo {1} caracteres")]
    public string Bairro { get; set; } = string.Empty;

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
    /// Contato
    /// </summary>
    [Display(Name = "Contato")]
    [StringLength(100, ErrorMessage = "Contato deve ter no maximo {1} caracteres")]
    public string Contato { get; set; } = string.Empty;

    /// <summary>
    /// Telefone Contato
    /// </summary>
    [Display(Name = "Telefone Contato")]
    [StringLength(20, ErrorMessage = "Telefone Contato deve ter no maximo {1} caracteres")]
    public string ContatoTelefone { get; set; } = string.Empty;

    /// <summary>
    /// E-mail Contato
    /// </summary>
    [Display(Name = "E-mail Contato")]
    [StringLength(100, ErrorMessage = "E-mail Contato deve ter no maximo {1} caracteres")]
    public string ContatoEmail { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo e obrigatorio")]
    public bool Ativo { get; set; }
}
