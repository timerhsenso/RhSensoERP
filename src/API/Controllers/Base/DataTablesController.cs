// =============================================================================
// RHSENSOERP - SHARED API
// =============================================================================
// Arquivo: src/Shared/RhSensoERP.Shared.Api/Controllers/DataTablesController.cs
// Descrição: Controller base com suporte a DataTables jQuery
// =============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RhSensoERP.Shared.Application.Interfaces;

namespace RhSensoERP.API.Controllers.Base;

/// <summary>
/// Request do DataTables jQuery.
/// </summary>
public class DataTablesRequest
{
    /// <summary>
    /// Draw counter para sincronização.
    /// </summary>
    public int Draw { get; set; }

    /// <summary>
    /// Índice do primeiro registro.
    /// </summary>
    public int Start { get; set; }

    /// <summary>
    /// Número de registros por página.
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Busca global.
    /// </summary>
    public DataTablesSearch? Search { get; set; }

    /// <summary>
    /// Ordenação.
    /// </summary>
    public List<DataTablesOrder>? Order { get; set; }

    /// <summary>
    /// Colunas.
    /// </summary>
    public List<DataTablesColumn>? Columns { get; set; }

    /// <summary>
    /// Converte para ServicePagedRequest.
    /// </summary>
    public ServicePagedRequest ToPagedRequest()
    {
        var request = new ServicePagedRequest
        {
            PageNumber = Start / Math.Max(Length, 1) + 1,
            PageSize = Length > 0 ? Length : 10,
            SearchTerm = Search?.Value
        };

        // Aplicar ordenação
        if (Order?.Any() == true && Columns?.Any() == true)
        {
            var orderInfo = Order.First();
            if (orderInfo.Column >= 0 && orderInfo.Column < Columns.Count)
            {
                var column = Columns[orderInfo.Column];
                request.SortField = column.Data ?? column.Name;
                request.SortDirection = orderInfo.Dir ?? "asc";
            }
        }

        return request;
    }
}

/// <summary>
/// Busca do DataTables.
/// </summary>
public class DataTablesSearch
{
    /// <summary>
    /// Valor da busca.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Se é regex.
    /// </summary>
    public bool Regex { get; set; }
}

/// <summary>
/// Ordenação do DataTables.
/// </summary>
public class DataTablesOrder
{
    /// <summary>
    /// Índice da coluna.
    /// </summary>
    public int Column { get; set; }

    /// <summary>
    /// Direção (asc/desc).
    /// </summary>
    public string? Dir { get; set; }
}

/// <summary>
/// Coluna do DataTables.
/// </summary>
public class DataTablesColumn
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
    /// Se é pesquisável.
    /// </summary>
    public bool Searchable { get; set; }

    /// <summary>
    /// Se é ordenável.
    /// </summary>
    public bool Orderable { get; set; }

    /// <summary>
    /// Busca específica da coluna.
    /// </summary>
    public DataTablesSearch? Search { get; set; }
}

/// <summary>
/// Response do DataTables jQuery.
/// </summary>
/// <typeparam name="T">Tipo dos dados</typeparam>
public class DataTablesResponse<T>
{
    /// <summary>
    /// Draw counter.
    /// </summary>
    public int Draw { get; set; }

    /// <summary>
    /// Total de registros sem filtro.
    /// </summary>
    public int RecordsTotal { get; set; }

    /// <summary>
    /// Total de registros após filtro.
    /// </summary>
    public int RecordsFiltered { get; set; }

    /// <summary>
    /// Dados.
    /// </summary>
    public IReadOnlyList<T> Data { get; set; } = new List<T>();

    /// <summary>
    /// Erro (se houver).
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Cria uma resposta do DataTables.
    /// </summary>
    public static DataTablesResponse<T> FromPagedResult(
        ServicePagedResult<T> result,
        int draw,
        int? totalWithoutFilter = null)
    {
        return new DataTablesResponse<T>
        {
            Draw = draw,
            RecordsTotal = totalWithoutFilter ?? result.TotalCount,
            RecordsFiltered = result.TotalCount,
            Data = result.Items
        };
    }

    /// <summary>
    /// Cria uma resposta de erro.
    /// </summary>
    public static DataTablesResponse<T> FromError(int draw, string error)
    {
        return new DataTablesResponse<T>
        {
            Draw = draw,
            Error = error,
            Data = new List<T>()
        };
    }
}

/// <summary>
/// Controller base com suporte a DataTables jQuery.
/// Herda de GenericApiController e adiciona endpoint para DataTables.
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
/// <typeparam name="TDto">Tipo do DTO</typeparam>
[ApiController]
[Produces("application/json")]
public abstract class DataTablesApiController<TEntity, TKey, TDto>
    : GenericApiController<TEntity, TKey, TDto>
    where TEntity : class
    where TKey : notnull
    where TDto : class
{
    /// <summary>
    /// Construtor.
    /// </summary>
    protected DataTablesApiController(
        ICrudService<TEntity, TKey, TDto> service,
        ILogger logger)
        : base(service, logger)
    {
    }

    /// <summary>
    /// Endpoint para DataTables jQuery (server-side processing).
    /// </summary>
    /// <param name="request">Request do DataTables</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Response formatado para DataTables</returns>
    [HttpPost("datatables")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<ActionResult<DataTablesResponse<TDto>>> DataTables(
        [FromBody] DataTablesRequest request,
        CancellationToken ct = default)
    {
        Logger.LogDebug("DataTables {Entity} - Start: {Start}, Length: {Length}",
            typeof(TEntity).Name, request.Start, request.Length);

        try
        {
            var pagedRequest = request.ToPagedRequest();
            var result = await Service.GetPagedAsync(pagedRequest, ct);

            // Para RecordsTotal, idealmente seria sem filtro
            // Por simplicidade, usamos o mesmo valor
            var response = DataTablesResponse<TDto>.FromPagedResult(result, request.Draw);

            return Ok(response);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro no DataTables {Entity}", typeof(TEntity).Name);
            return Ok(DataTablesResponse<TDto>.FromError(request.Draw, ex.Message));
        }
    }

    /// <summary>
    /// Endpoint GET para DataTables (alternativa ao POST).
    /// </summary>
    [HttpGet("datatables")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<ActionResult<DataTablesResponse<TDto>>> DataTablesGet(
        [FromQuery] int draw = 1,
        [FromQuery] int start = 0,
        [FromQuery] int length = 10,
        [FromQuery(Name = "search[value]")] string? searchValue = null,
        [FromQuery(Name = "order[0][column]")] int orderColumn = 0,
        [FromQuery(Name = "order[0][dir]")] string orderDir = "asc",
        CancellationToken ct = default)
    {
        var request = new DataTablesRequest
        {
            Draw = draw,
            Start = start,
            Length = length,
            Search = new DataTablesSearch { Value = searchValue },
            Order = new List<DataTablesOrder>
            {
                new DataTablesOrder { Column = orderColumn, Dir = orderDir }
            }
        };

        return await DataTables(request, ct);
    }
}
