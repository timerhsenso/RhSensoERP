// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapContatosEmergencia
// Module: ControleAcessoPortaria
// Data: 2025-12-28 20:54:55
// =============================================================================

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapContatosEmergencia;

/// <summary>
/// DTO de leitura para CapContatosEmergencia.
/// Compativel com backend: RhSensoERP.Modules.ControleAcessoPortaria.Application.DTOs.CapContatosEmergenciaDto
/// </summary>
public class CapContatosEmergenciaDto
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
    /// ID Funcionário Legado
    /// </summary>
    public int? IdFuncionarioLegado { get; set; }

    /// <summary>
    /// ID Colaborador Fornecedor
    /// </summary>
    public int? IdColaboradorFornecedor { get; set; }

    /// <summary>
    /// Nome
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// IdParentesco
    /// </summary>
    public int? IdParentesco { get; set; }

    /// <summary>
    /// Telefone Principal
    /// </summary>
    public string TelefonePrincipal { get; set; } = string.Empty;

    /// <summary>
    /// Telefone Secundário
    /// </summary>
    public string TelefoneSecundario { get; set; } = string.Empty;

    /// <summary>
    /// E-mail
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Endereço
    /// </summary>
    public string Endereco { get; set; } = string.Empty;

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
    /// Ordem Prioridade
    /// </summary>
    public int OrdemPrioridade { get; set; }

    /// <summary>
    /// Observações
    /// </summary>
    public string Observacoes { get; set; } = string.Empty;

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
