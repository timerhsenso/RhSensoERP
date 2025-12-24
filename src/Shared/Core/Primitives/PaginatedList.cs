// RhSensoERP.Shared.Core — PaginatedList<T>
// Finalidade: Padronizar respostas paginadas na aplicação.
// Uso: Envolver itens + metadados de paginação (TotalCount, PageIndex, PageSize).

using System;
using System.Collections.Generic;

namespace RhSensoERP.Shared.Core.Primitives;

/// <summary>
/// Contêiner imutável para resultados paginados.
/// </summary>
public sealed class PaginatedList<T>
{
    /// <summary>Itens da página corrente.</summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>Total de registros existentes (sem paginação).</summary>
    public int TotalCount { get; }

    /// <summary>Índice da página (1-based).</summary>
    public int PageIndex { get; }

    /// <summary>Tamanho da página.</summary>
    public int PageSize { get; }

    /// <summary>Total de páginas.</summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>Cria um novo resultado paginado.</summary>
    public PaginatedList(IReadOnlyList<T> items, int totalCount, int pageIndex, int pageSize)
    {
        Items = items ?? Array.Empty<T>();
        TotalCount = totalCount < 0 ? 0 : totalCount;
        PageIndex = pageIndex < 1 ? 1 : pageIndex;
        PageSize = pageSize < 1 ? 10 : pageSize;
    }
}
