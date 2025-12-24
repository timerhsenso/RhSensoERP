// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: CapFornecedores
// Module: ControleAcessoPortaria
// Data: 2025-12-24 01:02:36
// =============================================================================

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapFornecedores;

/// <summary>
/// DTO de leitura para CapFornecedores.
/// Compatível com backend: RhSensoERP.Modules.ControleAcessoPortaria.Application.DTOs.CapFornecedoresDto
/// </summary>
public class CapFornecedoresDto
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tenant Id
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// RazaoSocial
    /// </summary>
    public string RazaoSocial { get; set; } = string.Empty;

    /// <summary>
    /// NomeFantasia
    /// </summary>
    public string NomeFantasia { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ
    /// </summary>
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>
    /// CPF
    /// </summary>
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Telefone
    /// </summary>
    public string Telefone { get; set; } = string.Empty;

    /// <summary>
    /// Endereço
    /// </summary>
    public string Endereco { get; set; } = string.Empty;

    /// <summary>
    /// Número
    /// </summary>
    public string Numero { get; set; } = string.Empty;

    /// <summary>
    /// Complemento
    /// </summary>
    public string Complemento { get; set; } = string.Empty;

    /// <summary>
    /// Bairro
    /// </summary>
    public string Bairro { get; set; } = string.Empty;

    /// <summary>
    /// Cidade
    /// </summary>
    public string Cidade { get; set; } = string.Empty;

    /// <summary>
    /// ID UF
    /// </summary>
    public int? IdUf { get; set; }

    /// <summary>
    /// CEP
    /// </summary>
    public string Cep { get; set; } = string.Empty;

    /// <summary>
    /// Contato
    /// </summary>
    public string Contato { get; set; } = string.Empty;

    /// <summary>
    /// Telefone Contato
    /// </summary>
    public string ContatoTelefone { get; set; } = string.Empty;

    /// <summary>
    /// E-mail Contato
    /// </summary>
    public string ContatoEmail { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Created At Utc
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Created By User Id
    /// </summary>
    public Guid? CreatedByUserId { get; set; }

    /// <summary>
    /// Updated At Utc
    /// </summary>
    public DateTime UpdatedAtUtc { get; set; }

    /// <summary>
    /// Updated By User Id
    /// </summary>
    public Guid? UpdatedByUserId { get; set; }
}
