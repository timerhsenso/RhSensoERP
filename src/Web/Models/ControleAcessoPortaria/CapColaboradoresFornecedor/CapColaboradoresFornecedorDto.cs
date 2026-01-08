// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: CapColaboradoresFornecedor
// Module: ControleAcessoPortaria
// Data: 2026-01-07 21:15:52
// =============================================================================

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapColaboradoresFornecedor;

/// <summary>
/// DTO de leitura para Cadastro de Terceiros.
/// Compatível com backend: RhSensoERP.Modules.ControleAcessoPortaria.Application.DTOs.CapColaboradoresFornecedorDto
/// </summary>
public class CapColaboradoresFornecedorDto
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
    /// Fornecedor
    /// </summary>
    public int IdFornecedor { get; set; }

    /// <summary>
    /// Nome
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// CPF
    /// </summary>
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// RG
    /// </summary>
    public string Rg { get; set; } = string.Empty;

    /// <summary>
    /// E-mail
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Telefone
    /// </summary>
    public string Telefone { get; set; } = string.Empty;

    /// <summary>
    /// Data Nascimento
    /// </summary>
    public DateOnly? DataNascimento { get; set; }

    /// <summary>
    /// Genero
    /// </summary>
    public string Genero { get; set; } = string.Empty;

    /// <summary>
    /// Estado Civil
    /// </summary>
    public string EstadoCivil { get; set; } = string.Empty;

    /// <summary>
    /// Tipo Sanguineo
    /// </summary>
    public int? IdTipoSanguineo { get; set; }

    /// <summary>
    /// Endereco
    /// </summary>
    public string Endereco { get; set; } = string.Empty;

    /// <summary>
    /// Numero
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
    /// Uf
    /// </summary>
    public int? IdUf { get; set; }

    /// <summary>
    /// Cep
    /// </summary>
    public string Cep { get; set; } = string.Empty;

    /// <summary>
    /// Data Admissao
    /// </summary>
    public DateOnly DataAdmissao { get; set; }

    /// <summary>
    /// Data Demissao
    /// </summary>
    public DateOnly? DataDemissao { get; set; }

    /// <summary>
    /// Cargo
    /// </summary>
    public string Cargo { get; set; } = string.Empty;

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
    /// Campo 'RazaoSocial' da navegação Fornecedor.
    /// </summary>
    public string? FornecedorRazaoSocial { get; set; }

    /// <summary>
    /// Campo 'Descricao' da navegação Tiposanguineo.
    /// </summary>
    public string? TiposanguineoDescricao { get; set; }

    /// <summary>
    /// Campo 'Sigla' da navegação Uf.
    /// </summary>
    public string? UfSigla { get; set; }

}
