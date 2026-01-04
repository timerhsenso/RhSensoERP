// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: CapColaboradoresFornecedor
// Module: ControleAcessoPortaria
// Data: 2026-01-03 09:51:23
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapColaboradoresFornecedor;

/// <summary>
/// Request para atualização de CapColaboradoresFornecedor.
/// Compatível com backend: UpdateCapColaboradoresFornecedorRequest
/// </summary>
public class UpdateCapColaboradoresFornecedorRequest
{
    /// <summary>
    /// IdFornecedor
    /// </summary>
    [Display(Name = "IdFornecedor")]
    [Required(ErrorMessage = "IdFornecedor é obrigatório")]
    public int IdFornecedor { get; set; }

    /// <summary>
    /// Nome
    /// </summary>
    [Display(Name = "Nome")]
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(255, ErrorMessage = "Nome deve ter no máximo {1} caracteres")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// CPF
    /// </summary>
    [Display(Name = "CPF")]
    [Required(ErrorMessage = "CPF é obrigatório")]
    [StringLength(14, ErrorMessage = "CPF deve ter no máximo {1} caracteres")]
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// RG
    /// </summary>
    [Display(Name = "RG")]
    [StringLength(20, ErrorMessage = "RG deve ter no máximo {1} caracteres")]
    public string Rg { get; set; } = string.Empty;

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
    /// Data Nascimento
    /// </summary>
    [Display(Name = "Data Nascimento")]
    public DateOnly? DataNascimento { get; set; }

    /// <summary>
    /// Gênero
    /// </summary>
    [Display(Name = "Gênero")]
    [StringLength(20, ErrorMessage = "Gênero deve ter no máximo {1} caracteres")]
    public string Genero { get; set; } = string.Empty;

    /// <summary>
    /// Estado Civil
    /// </summary>
    [Display(Name = "Estado Civil")]
    [StringLength(50, ErrorMessage = "Estado Civil deve ter no máximo {1} caracteres")]
    public string EstadoCivil { get; set; } = string.Empty;

    /// <summary>
    /// ID Tipo Sanguíneo
    /// </summary>
    [Display(Name = "ID Tipo Sanguíneo")]
    public int? IdTipoSanguineo { get; set; }

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
    /// Data Admissão
    /// </summary>
    [Display(Name = "Data Admissão")]
    [Required(ErrorMessage = "Data Admissão é obrigatório")]
    public DateOnly DataAdmissao { get; set; }

    /// <summary>
    /// Data Demissão
    /// </summary>
    [Display(Name = "Data Demissão")]
    public DateOnly? DataDemissao { get; set; }

    /// <summary>
    /// Cargo
    /// </summary>
    [Display(Name = "Cargo")]
    [StringLength(100, ErrorMessage = "Cargo deve ter no máximo {1} caracteres")]
    public string Cargo { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo é obrigatório")]
    public bool Ativo { get; set; }
}
