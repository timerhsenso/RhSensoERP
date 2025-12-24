// src/Web/Services/Base/IBatchDeleteService.cs
using RhSensoERP.Web.Models.Common;

namespace RhSensoERP.Web.Services.Base;

/// <summary>
/// Interface para serviços que suportam exclusão em lote com resultado detalhado.
/// Implementar esta interface indica que o serviço pode fornecer informações
/// detalhadas sobre sucessos e falhas em operações de exclusão múltipla.
/// </summary>
/// <typeparam name="TKey">Tipo da chave primária da entidade.</typeparam>
public interface IBatchDeleteService<TKey>
{
    /// <summary>
    /// Exclui múltiplos registros em lote com resultado detalhado.
    /// </summary>
    /// <param name="ids">Lista de IDs dos registros a excluir.</param>
    /// <returns>
    /// Resultado detalhado da operação contendo:
    /// - Quantidade de sucessos
    /// - Quantidade de falhas
    /// - Lista de erros individuais
    /// </returns>
    Task<ApiResponse<BatchDeleteResultDto>> DeleteBatchAsync(IEnumerable<TKey> ids);
}
