// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TipoParentesco
// Module: Shared
// Data: 2026-03-02 17:59:26
// =============================================================================

namespace RhSensoERP.Web.Models.Shared.TipoParentesco;

/// <summary>
/// DTO de leitura para Tipo de Parentesco.
/// Compatível com backend: RhSensoERP.Modules.Shared.Application.DTOs.TipoParentescoDto
/// </summary>
public class TipoParentescoDto
{
    /// <summary>
    /// Id
    /// </summary>
    public byte Id { get; set; }

    /// <summary>
    /// Saas
    /// </summary>
    public Guid? IdSaas { get; set; }

    /// <summary>
    /// Código
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Ordem
    /// </summary>
    public byte Ordem { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    public bool Ativo { get; set; }
}
