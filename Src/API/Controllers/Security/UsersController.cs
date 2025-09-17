using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Application.Auth;
using RhSensoERP.Application.Security.Users.Dtos;
using MediatR;
using RhSensoERP.Application.Security.Users.Services;
using RhSensoERP.Core.Abstractions.Paging;
using RhSensoERP.Core.Shared;
using RhSensoERP.Core.Security.Permissions;

namespace RhSensoERP.API.Controllers.Security;

/// <summary>
/// Gerenciamento de usu�rios do sistema
/// </summary>
/// <remarks>
/// API para opera��es CRUD de usu�rios com controle de permiss�es baseado em JWT.
/// Todos os endpoints requerem autentica��o e permiss�es espec�ficas.
/// </remarks>
[ApiController]
[Route("api/v1/security/users")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Inicializa uma nova inst�ncia do controlador de usu�rios
    /// </summary>
    /// <param name="mediator">Inst�ncia do MediatR para processar comandos e queries</param>
    public UsersController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Lista usu�rios com pagina��o
    /// </summary>
    /// <param name="page">N�mero da p�gina (padr�o: 1)</param>
    /// <param name="pageSize">Itens por p�gina (padr�o: 20, m�ximo: 100)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Lista paginada de usu�rios</returns>
    /// <response code="200">Lista retornada com sucesso</response>
    /// <response code="401">Usu�rio n�o autenticado</response>
    /// <response code="403">Usu�rio n�o possui permiss�o SEC.Users.Read</response>
    [HttpGet]
    [Authorize]
    [HasPermission(PermissionConstants.SecUsersRead)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<UserListDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<UserListDto>>>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => ApiResponse<PagedResult<UserListDto>>.Ok(
            await _mediator.Send(new RhSensoERP.Application.Security.Users.Queries.GetUsersPagedQuery(page, Math.Min(pageSize, 100)), ct));

    /// <summary>
    /// Busca usu�rio por ID
    /// </summary>
    /// <param name="id">ID �nico do usu�rio</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Detalhes do usu�rio</returns>
    /// <response code="200">Usu�rio encontrado</response>
    /// <response code="404">Usu�rio n�o encontrado</response>
    [HttpGet("{id:guid}")]
    [Authorize]
    [HasPermission(PermissionConstants.SecUsersRead)]
    [ProducesResponseType(typeof(ApiResponse<UserDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserDetailDto?>>> GetById(Guid id, CancellationToken ct)
        => ApiResponse<UserDetailDto?>.Ok(await _mediator.Send(new RhSensoERP.Application.Security.Users.Queries.GetUserByIdQuery(id), ct));

    /// <summary>
    /// Cria um novo usu�rio
    /// </summary>
    /// <param name="dto">Dados do usu�rio</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>ID do usu�rio criado</returns>
    /// <response code="201">Usu�rio criado com sucesso</response>
    /// <response code="400">Dados inv�lidos ou usu�rio j� existe</response>
    [HttpPost]
    [Authorize]
    [HasPermission(PermissionConstants.SecUsersWrite)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] UserCreateDto dto, CancellationToken ct)
    {
        var id = await _mediator.Send(new RhSensoERP.Application.Security.Users.Commands.CreateUserCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id }, ApiResponse<object>.Ok(new { id }));
    }

    /// <summary>
    /// Atualiza um usu�rio existente
    /// </summary>
    /// <param name="id">ID do usu�rio</param>
    /// <param name="dto">Dados para atualiza��o</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Confirma��o da atualiza��o</returns>
    /// <response code="200">Usu�rio atualizado com sucesso</response>
    /// <response code="400">Dados inv�lidos</response>
    /// <response code="404">Usu�rio n�o encontrado</response>
    [HttpPut("{id:guid}")]
    [Authorize]
    [HasPermission(PermissionConstants.SecUsersWrite)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Update(Guid id, [FromBody] UserUpdateDto dto, CancellationToken ct)
    {
        await _mediator.Send(new RhSensoERP.Application.Security.Users.Commands.UpdateUserCommand(id, dto), ct);
        return ApiResponse<object>.Ok(new { id });
    }

    /// <summary>
    /// Remove um usu�rio (exclus�o l�gica)
    /// </summary>
    /// <param name="id">ID do usu�rio</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Confirma��o da exclus�o</returns>
    /// <response code="200">Usu�rio removido com sucesso</response>
    /// <response code="404">Usu�rio n�o encontrado</response>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [HasPermission(PermissionConstants.SecUsersDelete)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new RhSensoERP.Application.Security.Users.Commands.DeleteUserCommand(id), ct);
        return ApiResponse<object>.Ok(new { id });
    }
}