// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Sindicato
// Module: AdministracaoPessoal
// Data: 2026-02-28 21:56:09
// =============================================================================

namespace RhSensoERP.Web.Models.AdministracaoPessoal.Sindicato;

/// <summary>
/// DTO de leitura para Sindicato.
/// Compatível com backend: RhSensoERP.Modules.AdministracaoPessoal.Application.DTOs.SindicatoDto
/// </summary>
public class SindicatoDto
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Código
    /// </summary>
    public string Cdsindicat { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    public string Dcsindicat { get; set; } = string.Empty;

    /// <summary>
    /// Endereço
    /// </summary>
    public string Dcendereco { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ
    /// </summary>
    public string Cgcsindicat { get; set; } = string.Empty;

    /// <summary>
    /// Código Entidade
    /// </summary>
    public string CdentIdade { get; set; } = string.Empty;

    /// <summary>
    /// Data Base
    /// </summary>
    public string DataBase { get; set; } = string.Empty;

    /// <summary>
    /// Tipo
    /// </summary>
    public int? Fltipo { get; set; }

    /// <summary>
    /// Tabela Base
    /// </summary>
    public string Cdtabbase { get; set; } = string.Empty;
}
