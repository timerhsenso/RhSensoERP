// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: SituacaoColaborador
// Module: AdministracaoPessoal
// Data: 2026-02-28 22:05:47
// =============================================================================

namespace RhSensoERP.Web.Models.AdministracaoPessoal.SituacaoColaborador;

/// <summary>
/// DTO de leitura para Situação do Colaborador.
/// Compatível com backend: RhSensoERP.Modules.AdministracaoPessoal.Application.DTOs.SituacaoColaboradorDto
/// </summary>
public class SituacaoColaboradorDto
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Código
    /// </summary>
    public string Cdsituacao { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    public string Dcsituacao { get; set; } = string.Empty;

    /// <summary>
    /// É Demissão
    /// </summary>
    public string Fldemissao { get; set; } = string.Empty;

    /// <summary>
    /// É Afastamento
    /// </summary>
    public string FlafaStame { get; set; } = string.Empty;

    /// <summary>
    /// Dias Benefício
    /// </summary>
    public int? Qtdiasbene { get; set; }

    /// <summary>
    /// Dias Previdência
    /// </summary>
    public int? Qtdiasprev { get; set; }

    /// <summary>
    /// Código FGTS
    /// </summary>
    public string Cdfgts { get; set; } = string.Empty;

    /// <summary>
    /// Código SEFIP
    /// </summary>
    public string Cdsefip { get; set; } = string.Empty;

    /// <summary>
    /// Código SEFIP 2
    /// </summary>
    public string Cdsefip2 { get; set; } = string.Empty;

    /// <summary>
    /// Perde Férias
    /// </summary>
    public string Flpferias { get; set; } = string.Empty;
}
