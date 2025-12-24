// =============================================================================
// NOVO DTO: Response para validação específica de permissão
// src/Identity/Application/DTOs/Permissoes/ValidarPermissaoResponse.cs
// =============================================================================

namespace RhSensoERP.Identity.Application.DTOs.Permissoes;

/// <summary>
/// Response da validação de permissão específica.
/// Retorna se o usuário tem ou não a permissão solicitada.
/// </summary>
public sealed class ValidarPermissaoResponse
{
    /// <summary>
    /// Indica se o usuário tem a permissão solicitada
    /// </summary>
    public bool TemPermissao { get; set; }

    /// <summary>
    /// Código do usuário validado
    /// </summary>
    public string CdUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Código do sistema validado
    /// </summary>
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Código da função validada
    /// </summary>
    public string CdFuncao { get; set; } = string.Empty;

    /// <summary>
    /// Ação validada (I, A, E, C)
    /// </summary>
    public char Acao { get; set; }

    /// <summary>
    /// Descrição da ação validada
    /// </summary>
    public string DescricaoAcao { get; set; } = string.Empty;

    /// <summary>
    /// Motivo da negação (preenchido apenas quando TemPermissao = false)
    /// </summary>
    public string? Motivo { get; set; }

    /// <summary>
    /// Todas as ações que o usuário possui para esta função
    /// Exemplo: "IAEC", "IAC", "C"
    /// </summary>
    public string? AcoesDisponiveis { get; set; }
}
