// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: CapFornecedores
// Module: ControleAcessoPortaria
// Data: 2025-12-24 01:02:36
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapFornecedores;

/// <summary>
/// Request para atualização de CapFornecedores.
/// Compatível com backend: UpdateCapFornecedoresRequest
/// </summary>
public class UpdateCapFornecedoresRequest
{
    /// <summary>
    /// Tenant Id
    /// </summary>
    [Display(Name = "Tenant Id")]
    [Required(ErrorMessage = "Tenant Id é obrigatório")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// RazaoSocial
    /// </summary>
    [Display(Name = "RazaoSocial")]
    [StringLength(255, ErrorMessage = "RazaoSocial deve ter no máximo {1} caracteres")]
    public string RazaoSocial { get; set; } = string.Empty;

    /// <summary>
    /// NomeFantasia
    /// </summary>
    [Display(Name = "NomeFantasia")]
    [StringLength(255, ErrorMessage = "NomeFantasia deve ter no máximo {1} caracteres")]
    public string NomeFantasia { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ
    /// </summary>
    [Display(Name = "CNPJ")]
    [StringLength(18, ErrorMessage = "CNPJ deve ter no máximo {1} caracteres")]
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>
    /// CPF
    /// </summary>
    [Display(Name = "CPF")]
    [StringLength(14, ErrorMessage = "CPF deve ter no máximo {1} caracteres")]
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// Email
    /// </summary>
    [Display(Name = "Email")]
    [StringLength(100, ErrorMessage = "Email deve ter no máximo {1} caracteres")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Telefone
    /// </summary>
    [Display(Name = "Telefone")]
    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo {1} caracteres")]
    public string Telefone { get; set; } = string.Empty;

    /// <summary>
    /// Endereço
    /// </summary>
    [Display(Name = "Endereço")]
    [StringLength(500, ErrorMessage = "Endereço deve ter no máximo {1} caracteres")]
    public string Endereco { get; set; } = string.Empty;

    /// <summary>
    /// Número
    /// </summary>
    [Display(Name = "Número")]
    [StringLength(20, ErrorMessage = "Número deve ter no máximo {1} caracteres")]
    public string Numero { get; set; } = string.Empty;

    /// <summary>
    /// Complemento
    /// </summary>
    [Display(Name = "Complemento")]
    [StringLength(200, ErrorMessage = "Complemento deve ter no máximo {1} caracteres")]
    public string Complemento { get; set; } = string.Empty;

    /// <summary>
    /// Bairro
    /// </summary>
    [Display(Name = "Bairro")]
    [StringLength(100, ErrorMessage = "Bairro deve ter no máximo {1} caracteres")]
    public string Bairro { get; set; } = string.Empty;

    /// <summary>
    /// Cidade
    /// </summary>
    [Display(Name = "Cidade")]
    [StringLength(100, ErrorMessage = "Cidade deve ter no máximo {1} caracteres")]
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
    [StringLength(10, ErrorMessage = "CEP deve ter no máximo {1} caracteres")]
    public string Cep { get; set; } = string.Empty;

    /// <summary>
    /// Contato
    /// </summary>
    [Display(Name = "Contato")]
    [StringLength(100, ErrorMessage = "Contato deve ter no máximo {1} caracteres")]
    public string Contato { get; set; } = string.Empty;

    /// <summary>
    /// Telefone Contato
    /// </summary>
    [Display(Name = "Telefone Contato")]
    [StringLength(20, ErrorMessage = "Telefone Contato deve ter no máximo {1} caracteres")]
    public string ContatoTelefone { get; set; } = string.Empty;

    /// <summary>
    /// E-mail Contato
    /// </summary>
    [Display(Name = "E-mail Contato")]
    [StringLength(100, ErrorMessage = "E-mail Contato deve ter no máximo {1} caracteres")]
    public string ContatoEmail { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo é obrigatório")]
    public bool Ativo { get; set; }

    /// <summary>
    /// Created At Utc
    /// </summary>
    [Display(Name = "Created At Utc")]
    [Required(ErrorMessage = "Created At Utc é obrigatório")]
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Created By User Id
    /// </summary>
    [Display(Name = "Created By User Id")]
    public Guid? CreatedByUserId { get; set; }

    /// <summary>
    /// Updated At Utc
    /// </summary>
    [Display(Name = "Updated At Utc")]
    [Required(ErrorMessage = "Updated At Utc é obrigatório")]
    public DateTime UpdatedAtUtc { get; set; }

    /// <summary>
    /// Updated By User Id
    /// </summary>
    [Display(Name = "Updated By User Id")]
    public Guid? UpdatedByUserId { get; set; }
}
