// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: CapColaboradoresFornecedor
// Module: ControleAcessoPortaria
// Data: 2026-01-07 21:15:52
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapColaboradoresFornecedor;

/// <summary>
/// Request para criação de Cadastro de Terceiros.
/// Compatível com backend: CreateCapColaboradoresFornecedorRequest
/// </summary>
public class CreateCapColaboradoresFornecedorRequest
{
    /// <summary>
    /// Fornecedor
    /// </summary>
    [Display(Name = "Fornecedor")]
    [Required(ErrorMessage = "Fornecedor é obrigatório")]
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
    /// E-mail
    /// </summary>
    [Display(Name = "E-mail")]
    [StringLength(100, ErrorMessage = "E-mail deve ter no máximo {1} caracteres")]
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
    /// Genero
    /// </summary>
    [Display(Name = "Genero")]
    [StringLength(20, ErrorMessage = "Genero deve ter no máximo {1} caracteres")]
    public string Genero { get; set; } = string.Empty;

    /// <summary>
    /// Estado Civil
    /// </summary>
    [Display(Name = "Estado Civil")]
    [StringLength(50, ErrorMessage = "Estado Civil deve ter no máximo {1} caracteres")]
    public string EstadoCivil { get; set; } = string.Empty;

    /// <summary>
    /// Tipo Sanguineo
    /// </summary>
    [Display(Name = "Tipo Sanguineo")]
    public int? IdTipoSanguineo { get; set; }

    /// <summary>
    /// Endereco
    /// </summary>
    [Display(Name = "Endereco")]
    [StringLength(500, ErrorMessage = "Endereco deve ter no máximo {1} caracteres")]
    public string Endereco { get; set; } = string.Empty;

    /// <summary>
    /// Numero
    /// </summary>
    [Display(Name = "Numero")]
    [StringLength(20, ErrorMessage = "Numero deve ter no máximo {1} caracteres")]
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
    /// Uf
    /// </summary>
    [Display(Name = "Uf")]
    public int? IdUf { get; set; }

    /// <summary>
    /// Cep
    /// </summary>
    [Display(Name = "Cep")]
    [StringLength(10, ErrorMessage = "Cep deve ter no máximo {1} caracteres")]
    public string Cep { get; set; } = string.Empty;

    /// <summary>
    /// Data Admissao
    /// </summary>
    [Display(Name = "Data Admissao")]
    [Required(ErrorMessage = "Data Admissao é obrigatório")]
    public DateOnly DataAdmissao { get; set; }

    /// <summary>
    /// Data Demissao
    /// </summary>
    [Display(Name = "Data Demissao")]
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
