// =============================================================================
// DTO: Response para toggle de permissão
// src/Identity/Application/DTOs/Permissoes/TogglePermissaoResponse.cs
// =============================================================================

namespace RhSensoERP.Identity.Application.DTOs.Permissoes;

/// <summary>
/// Response do toggle de permissão.
/// </summary>
public sealed class TogglePermissaoResponse
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem descritiva do resultado
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// String de ações após a atualização (ex: "IAC", "IAEC")
    /// </summary>
    public string CdAcoesAtualizado { get; set; } = string.Empty;

    /// <summary>
    /// Código do grupo que foi alterado
    /// </summary>
    public string? CdGrUser { get; set; }
}