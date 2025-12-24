// =============================================================================
// RHSENSOERP - SHARED API
// =============================================================================
// Arquivo: src/Shared/RhSensoERP.Shared.Api/Controllers/GenericApiController.cs
// Descrição: Controller genérico com endpoints CRUD automáticos
// =============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RhSensoERP.Shared.Application.Interfaces;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.API.Controllers.Base;

/// <summary>
/// Controller base genérico com operações CRUD.
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
/// <typeparam name="TDto">Tipo do DTO</typeparam>
[ApiController]
[Produces("application/json")]
public abstract class GenericApiController<TEntity, TKey, TDto> : ControllerBase
    where TEntity : class
    where TKey : notnull
    where TDto : class
{
    protected readonly ICrudService<TEntity, TKey, TDto> Service;
    protected readonly ILogger Logger;

    /// <summary>
    /// Construtor.
    /// </summary>
    protected GenericApiController(
        ICrudService<TEntity, TKey, TDto> service,
        ILogger logger)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Lista todos os registros com paginação.
    /// </summary>
    /// <param name="pageNumber">Número da página (1-based)</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <param name="sortField">Campo para ordenação</param>
    /// <param name="sortDirection">Direção (asc/desc)</param>
    /// <param name="searchTerm">Termo de busca global</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Lista paginada de registros</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<ActionResult<ServicePagedResult<TDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortField = null,
        [FromQuery] string sortDirection = "asc",
        [FromQuery] string? searchTerm = null,
        CancellationToken ct = default)
    {
        Logger.LogDebug("GET {Entity} - Página: {Page}, Tamanho: {Size}",
            typeof(TEntity).Name, pageNumber, pageSize);

        var request = new ServicePagedRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortField = sortField,
            SortDirection = sortDirection,
            SearchTerm = searchTerm
        };

        var result = await Service.GetPagedAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// Obtém um registro pelo ID.
    /// </summary>
    /// <param name="id">ID do registro</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Registro encontrado ou 404</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<TDto>> GetById(TKey id, CancellationToken ct = default)
    {
        Logger.LogDebug("GET {Entity}/{Id}", typeof(TEntity).Name, id);

        var dto = await Service.GetByIdAsync(id, ct);
        if (dto == null)
        {
            Logger.LogWarning("{Entity} não encontrado: {Id}", typeof(TEntity).Name, id);
            return NotFound(new { message = $"Registro não encontrado: {id}" });
        }

        return Ok(dto);
    }

    /// <summary>
    /// Cria um novo registro.
    /// </summary>
    /// <param name="dto">Dados do registro</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Registro criado</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<TDto>> Create([FromBody] TDto dto, CancellationToken ct = default)
    {
        Logger.LogDebug("POST {Entity}", typeof(TEntity).Name);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await Service.CreateAsync(dto, ct);

        if (!result.IsSuccess)
        {
            Logger.LogWarning("Erro ao criar {Entity}: {Error}",
                typeof(TEntity).Name, result.Error.Message);
            return BadRequest(new { message = result.Error.Message, code = result.Error.Code });
        }

        // Tentar obter o ID do DTO criado para retornar CreatedAtAction
        var createdDto = result.Value!;
        var idProperty = typeof(TDto).GetProperty("Id")
            ?? typeof(TDto).GetProperty($"Cd{typeof(TEntity).Name}")
            ?? typeof(TDto).GetProperties().FirstOrDefault(p =>
                p.Name.StartsWith("Cd", StringComparison.OrdinalIgnoreCase) ||
                p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase));

        if (idProperty != null)
        {
            var createdId = idProperty.GetValue(createdDto);
            return CreatedAtAction(nameof(GetById), new { id = createdId }, createdDto);
        }

        return Created(string.Empty, createdDto);
    }

    /// <summary>
    /// Atualiza um registro existente.
    /// </summary>
    /// <param name="id">ID do registro</param>
    /// <param name="dto">Dados atualizados</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Registro atualizado</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<TDto>> Update(TKey id, [FromBody] TDto dto, CancellationToken ct = default)
    {
        Logger.LogDebug("PUT {Entity}/{Id}", typeof(TEntity).Name, id);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await Service.UpdateAsync(id, dto, ct);

        if (!result.IsSuccess)
        {
            if (result.Error.Code == "NotFound")
            {
                return NotFound(new { message = result.Error.Message });
            }

            Logger.LogWarning("Erro ao atualizar {Entity}/{Id}: {Error}",
                typeof(TEntity).Name, id, result.Error.Message);
            return BadRequest(new { message = result.Error.Message, code = result.Error.Code });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Remove um registro.
    /// </summary>
    /// <param name="id">ID do registro</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>204 No Content ou erro</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<IActionResult> Delete(TKey id, CancellationToken ct = default)
    {
        Logger.LogDebug("DELETE {Entity}/{Id}", typeof(TEntity).Name, id);

        var result = await Service.DeleteAsync(id, ct);

        if (!result.IsSuccess)
        {
            if (result.Error.Code == "NotFound")
            {
                return NotFound(new { message = result.Error.Message });
            }

            Logger.LogWarning("Erro ao excluir {Entity}/{Id}: {Error}",
                typeof(TEntity).Name, id, result.Error.Message);
            return BadRequest(new { message = result.Error.Message, code = result.Error.Code });
        }

        return NoContent();
    }

    /// <summary>
    /// Verifica se um registro existe.
    /// </summary>
    /// <param name="id">ID do registro</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>200 OK com true/false</returns>
    [HttpHead("{id}")]
    [HttpGet("{id}/exists")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<ActionResult<bool>> Exists(TKey id, CancellationToken ct = default)
    {
        var exists = await Service.ExistsAsync(id, ct);
        return Ok(exists);
    }

    /// <summary>
    /// Conta o total de registros.
    /// </summary>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Total de registros</returns>
    [HttpGet("count")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<ActionResult<int>> Count(CancellationToken ct = default)
    {
        var count = await Service.CountAsync(ct);
        return Ok(count);
    }
}

/// <summary>
/// Controller base genérico com DTOs separados para Create e Update.
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
/// <typeparam name="TDto">Tipo do DTO de leitura</typeparam>
/// <typeparam name="TCreateDto">Tipo do DTO de criação</typeparam>
/// <typeparam name="TUpdateDto">Tipo do DTO de atualização</typeparam>
[ApiController]
[Produces("application/json")]
public abstract class GenericApiController<TEntity, TKey, TDto, TCreateDto, TUpdateDto> : ControllerBase
    where TEntity : class
    where TKey : notnull
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    protected readonly ICrudService<TEntity, TKey, TDto, TCreateDto, TUpdateDto> Service;
    protected readonly ILogger Logger;

    /// <summary>
    /// Construtor.
    /// </summary>
    protected GenericApiController(
        ICrudService<TEntity, TKey, TDto, TCreateDto, TUpdateDto> service,
        ILogger logger)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Lista todos os registros com paginação.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<ActionResult<ServicePagedResult<TDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortField = null,
        [FromQuery] string sortDirection = "asc",
        [FromQuery] string? searchTerm = null,
        CancellationToken ct = default)
    {
        Logger.LogDebug("GET {Entity} - Página: {Page}, Tamanho: {Size}",
            typeof(TEntity).Name, pageNumber, pageSize);

        var request = new ServicePagedRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortField = sortField,
            SortDirection = sortDirection,
            SearchTerm = searchTerm
        };

        var result = await Service.GetPagedAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// Obtém um registro pelo ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<TDto>> GetById(TKey id, CancellationToken ct = default)
    {
        Logger.LogDebug("GET {Entity}/{Id}", typeof(TEntity).Name, id);

        var dto = await Service.GetByIdAsync(id, ct);
        if (dto == null)
        {
            return NotFound(new { message = $"Registro não encontrado: {id}" });
        }

        return Ok(dto);
    }

    /// <summary>
    /// Cria um novo registro.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<TDto>> Create([FromBody] TCreateDto dto, CancellationToken ct = default)
    {
        Logger.LogDebug("POST {Entity}", typeof(TEntity).Name);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await Service.CreateAsync(dto, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error.Message, code = result.Error.Code });
        }

        return Created(string.Empty, result.Value);
    }

    /// <summary>
    /// Atualiza um registro existente.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<TDto>> Update(TKey id, [FromBody] TUpdateDto dto, CancellationToken ct = default)
    {
        Logger.LogDebug("PUT {Entity}/{Id}", typeof(TEntity).Name, id);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await Service.UpdateAsync(id, dto, ct);

        if (!result.IsSuccess)
        {
            if (result.Error.Code == "NotFound")
            {
                return NotFound(new { message = result.Error.Message });
            }

            return BadRequest(new { message = result.Error.Message, code = result.Error.Code });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Remove um registro.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<IActionResult> Delete(TKey id, CancellationToken ct = default)
    {
        Logger.LogDebug("DELETE {Entity}/{Id}", typeof(TEntity).Name, id);

        var result = await Service.DeleteAsync(id, ct);

        if (!result.IsSuccess)
        {
            if (result.Error.Code == "NotFound")
            {
                return NotFound(new { message = result.Error.Message });
            }

            return BadRequest(new { message = result.Error.Message, code = result.Error.Code });
        }

        return NoContent();
    }

    /// <summary>
    /// Verifica se um registro existe.
    /// </summary>
    [HttpHead("{id}")]
    [HttpGet("{id}/exists")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<ActionResult<bool>> Exists(TKey id, CancellationToken ct = default)
    {
        var exists = await Service.ExistsAsync(id, ct);
        return Ok(exists);
    }

    /// <summary>
    /// Conta o total de registros.
    /// </summary>
    [HttpGet("count")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<ActionResult<int>> Count(CancellationToken ct = default)
    {
        var count = await Service.CountAsync(ct);
        return Ok(count);
    }
}
