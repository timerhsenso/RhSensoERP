// =============================================================================
// DTO: Request para toggle de permissão
// src/Identity/Application/DTOs/Permissoes/TogglePermissaoRequest.cs
// =============================================================================

namespace RhSensoERP.Identity.Application.DTOs.Permissoes;

/// <summary>
/// Request para habilitar/desabilitar uma permissão específica.
/// </summary>
public sealed class TogglePermissaoRequest
{
    /// <summary>
    /// Código do usuário (para identificar o grupo)
    /// </summary>
    public string CdUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Código do sistema (ex: RHU, FIN)
    /// </summary>
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Código da função
    /// </summary>
    public string CdFuncao { get; set; } = string.Empty;

    /// <summary>
    /// Ação a ser alterada: I (Incluir), A (Alterar), E (Excluir), C (Consultar)
    /// </summary>
    public char Acao { get; set; }

    /// <summary>
    /// True para habilitar, False para desabilitar
    /// </summary>
    public bool Enabled { get; set; }
}