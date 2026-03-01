// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: AgenciaBancaria
// Module: AdministracaoPessoal
// Data: 2026-02-28 20:05:58
// =============================================================================

namespace RhSensoERP.Web.Models.AdministracaoPessoal.AgenciaBancaria;

/// <summary>
/// DTO de leitura para Tabela de Agências.
/// Compatível com backend: RhSensoERP.Modules.AdministracaoPessoal.Application.DTOs.AgenciaBancariaDto
/// </summary>
public class AgenciaBancariaDto
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Código Banco
    /// </summary>
    public string Cdbanco { get; set; } = string.Empty;

    /// <summary>
    /// Código Agência
    /// </summary>
    public string Cdagencia { get; set; } = string.Empty;

    /// <summary>
    /// Dígito
    /// </summary>
    public string Dvagencia { get; set; } = string.Empty;

    /// <summary>
    /// Nome
    /// </summary>
    public string Nmagencia { get; set; } = string.Empty;

    /// <summary>
    /// Código Município
    /// </summary>
    public string Cdmunicip { get; set; } = string.Empty;

    /// <summary>
    /// Conta
    /// </summary>
    public string Noconta { get; set; } = string.Empty;

    /// <summary>
    /// Idbanco
    /// </summary>
    public Guid? Idbanco { get; set; }
}
