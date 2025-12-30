// =============================================================================
// RHSENSOERP - BATCH DELETE RESULT
// =============================================================================
// Arquivo: src/Shared/Core/Common/BatchDeleteResult.cs
// Versão: 1.0
// Descrição: DTO para resultado de exclusão em lote com suporte a FK violations
// =============================================================================

namespace RhSensoERP.Shared.Core.Common;

/// <summary>
/// Resultado de operação de exclusão em lote.
/// </summary>
public sealed class BatchDeleteResult
{
    /// <summary>
    /// Total de registros processados.
    /// </summary>
    public int TotalProcessados { get; set; }

    /// <summary>
    /// Quantidade de registros excluídos com sucesso.
    /// </summary>
    public int TotalDeletados { get; set; }

    /// <summary>
    /// Quantidade de registros que não puderam ser excluídos.
    /// </summary>
    public int TotalNaoDeletados { get; set; }

    /// <summary>
    /// Lista de erros detalhados.
    /// </summary>
    public List<string> Erros { get; set; } = new();

    /// <summary>
    /// Indica se pelo menos um registro foi deletado com sucesso.
    /// </summary>
    public bool Sucesso => TotalDeletados > 0;

    /// <summary>
    /// Indica se todas as exclusões foram bem-sucedidas.
    /// </summary>
    public bool TodosSucesso => TotalNaoDeletados == 0 && TotalDeletados > 0;
}