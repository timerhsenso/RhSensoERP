// src/Web/Models/Base/DataTableRequest.cs

namespace RhSensoERP.Web.Models.Base;

/// <summary>
/// Request padrão do DataTables (server-side processing).
/// </summary>
public sealed class DataTableRequest
{
    /// <summary>
    /// Contador de requisições (usado pelo DataTables para sincronização).
    /// </summary>
    public int Draw { get; set; }

    /// <summary>
    /// Índice do primeiro registro a ser retornado.
    /// </summary>
    public int Start { get; set; }

    /// <summary>
    /// Quantidade de registros por página.
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Informações de busca global.
    /// </summary>
    public DataTableSearch? Search { get; set; }

    /// <summary>
    /// Informações de ordenação.
    /// </summary>
    public List<DataTableOrder>? Order { get; set; }

    /// <summary>
    /// Informações das colunas.
    /// </summary>
    public List<DataTableColumn>? Columns { get; set; }
}

/// <summary>
/// Informações de busca do DataTables.
/// </summary>
public sealed class DataTableSearch
{
    /// <summary>
    /// Valor da busca.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Indica se a busca usa regex.
    /// </summary>
    public bool Regex { get; set; }
}

/// <summary>
/// Informações de ordenação do DataTables.
/// </summary>
public sealed class DataTableOrder
{
    /// <summary>
    /// Índice da coluna a ser ordenada.
    /// </summary>
    public int Column { get; set; }

    /// <summary>
    /// Direção da ordenação (asc ou desc).
    /// </summary>
    public string Dir { get; set; } = "asc";
}

/// <summary>
/// Informações de coluna do DataTables.
/// </summary>
public sealed class DataTableColumn
{
    /// <summary>
    /// Nome do campo de dados.
    /// </summary>
    public string? Data { get; set; }

    /// <summary>
    /// Nome da coluna.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Indica se a coluna é pesquisável.
    /// </summary>
    public bool Searchable { get; set; }

    /// <summary>
    /// Indica se a coluna é ordenável.
    /// </summary>
    public bool Orderable { get; set; }

    /// <summary>
    /// Informações de busca específica da coluna.
    /// </summary>
    public DataTableSearch? Search { get; set; }
}

/// <summary>
/// Response padrão do DataTables (server-side processing).
/// </summary>
/// <typeparam name="T">Tipo do item da lista</typeparam>
public sealed class DataTableResponse<T>
{
    /// <summary>
    /// Contador de requisições (mesmo valor do request).
    /// </summary>
    public int Draw { get; set; }

    /// <summary>
    /// Total de registros sem filtro.
    /// </summary>
    public int RecordsTotal { get; set; }

    /// <summary>
    /// Total de registros com filtro aplicado.
    /// </summary>
    public int RecordsFiltered { get; set; }

    /// <summary>
    /// Lista de dados da página atual.
    /// </summary>
    public List<T> Data { get; set; } = new();

    /// <summary>
    /// Mensagem de erro (se houver).
    /// </summary>
    public string? Error { get; set; }
}
