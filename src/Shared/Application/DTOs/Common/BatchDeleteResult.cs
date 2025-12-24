// =============================================================================
// RHSENSOERP - BATCH DELETE RESULT
// =============================================================================
// Arquivo: src/Shared/Application/DTOs/Common/BatchDeleteResult.cs
// Descrição: DTO comum para resultado de exclusão em lote
// =============================================================================

namespace RhSensoERP.Shared.Application.DTOs.Common;

/// <summary>
/// Resultado de operação de exclusão em lote.
/// </summary>
public sealed record BatchDeleteResult
{
    /// <summary>
    /// Quantidade de registros excluídos com sucesso.
    /// </summary>
    public int SuccessCount { get; init; }

    /// <summary>
    /// Quantidade de registros que falharam.
    /// </summary>
    public int FailureCount { get; init; }

    /// <summary>
    /// Total de registros processados.
    /// </summary>
    public int TotalCount => SuccessCount + FailureCount;

    /// <summary>
    /// Lista de erros por código.
    /// </summary>
    public List<BatchDeleteError> Errors { get; init; } = [];

    /// <summary>
    /// Indica se todas as exclusões foram bem-sucedidas.
    /// </summary>
    public bool AllSucceeded => FailureCount == 0;
}

/// <summary>
/// Erro de exclusão individual.
/// </summary>
public sealed record BatchDeleteError(string Code, string Message);