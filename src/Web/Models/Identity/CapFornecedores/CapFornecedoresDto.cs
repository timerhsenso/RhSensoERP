// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: CapFornecedores
// Module: Identity
// Data: 2026-01-07 21:24:22
// =============================================================================

namespace RhSensoERP.Web.Models.Identity.CapFornecedores;

/// <summary>
/// DTO de leitura para Cadastro de Fonecedores.
/// Compatível com backend: RhSensoERP.Modules.Identity.Application.DTOs.CapFornecedoresDto
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
    /// Razão Social
    /// </summary>
    public string RazaoSocial { get; set; } = string.Empty;

    /// <summary>
    /// Nome Fantasia
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
    /// E-mail
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

    // =========================================================================
    // PROPRIEDADES DE NAVEGAÇÃO (campos de entidades relacionadas)
    // =========================================================================

    /// <summary>
    /// Campo 'Sigla' da navegação Uf.
    /// </summary>
    public string? UfSigla { get; set; }

}
