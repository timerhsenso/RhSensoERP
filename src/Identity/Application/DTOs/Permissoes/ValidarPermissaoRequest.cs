// =============================================================================
// NOVO DTO: Request para validação específica de permissão
// src/Identity/Application/DTOs/Permissoes/ValidarPermissaoRequest.cs
// =============================================================================

namespace RhSensoERP.Identity.Application.DTOs.Permissoes;

/// <summary>
/// Request para validação de permissão específica de um usuário.
/// Permite validar se o usuário tem uma ação específica (I, A, E, C) 
/// em uma função de um sistema.
/// </summary>
public sealed class ValidarPermissaoRequest
{
    /// <summary>
    /// Código do usuário (obrigatório)
    /// </summary>
    public required string CdUsuario { get; set; }

    /// <summary>
    /// Código do sistema (obrigatório)
    /// Exemplo: "RHU", "FIN", "EST"
    /// </summary>
    public required string CdSistema { get; set; }

    /// <summary>
    /// Código da função/tela (obrigatório)
    /// Exemplo: "SEG_FM_TSISTEMA", "RHU_FM_FUNCIONARIO"
    /// </summary>
    public required string CdFuncao { get; set; }

    /// <summary>
    /// Ação a ser validada (obrigatório)
    /// I = Incluir
    /// A = Alterar
    /// E = Excluir
    /// C = Consultar
    /// </summary>
    public required char Acao { get; set; }
}
