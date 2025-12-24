// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: TreTiposTreinamento
// Module: TreinamentoDesenvolvimento
// Data: 2025-12-22 12:51:16
// =============================================================================

namespace RhSensoERP.Web.Models.TreinamentoDesenvolvimento.TreTiposTreinamento;

/// <summary>
/// DTO de leitura para Tipos de Treinamento.
/// Compatível com backend: RhSensoERP.Modules.TreinamentoDesenvolvimento.Application.DTOs.TreTiposTreinamentoDto
/// </summary>
public class TreTiposTreinamentoDto
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Saas
    /// </summary>
    public int IdSaas { get; set; }

    /// <summary>
    /// Nome
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Codigo Nr
    /// </summary>
    public string CodigoNr { get; set; } = string.Empty;

    /// <summary>
    /// Dias Prazo Val Idade
    /// </summary>
    public int? DiasPrazoValIdade { get; set; }

    /// <summary>
    /// Obrigatorio
    /// </summary>
    public bool Obrigatorio { get; set; }

    /// <summary>
    /// Aplicavel A
    /// </summary>
    public string AplicavelA { get; set; } = string.Empty;

    /// <summary>
    /// Carga Horaria
    /// </summary>
    public int? CargaHoraria { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Data Criacao
    /// </summary>
    public DateTime? DataCriacao { get; set; }

    /// <summary>
    /// Usuario Criacao
    /// </summary>
    public string UsuarioCriacao { get; set; } = string.Empty;

    /// <summary>
    /// Data Atualizacao
    /// </summary>
    public DateTime? DataAtualizacao { get; set; }

    /// <summary>
    /// Usuario Atualizacao
    /// </summary>
    public string UsuarioAtualizacao { get; set; } = string.Empty;
}
