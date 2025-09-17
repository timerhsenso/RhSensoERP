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
/// Gerenciamento de usuários do sistema
/// </summary>
/// <remarks>
/// API para operações CRUD de usuários com controle de permissões baseado em JWT.
/// Todos os endpoints requerem autenticação e permissões específicas.
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
    /// Inicializa uma nova instância do controlador de usuários
    /// </summary>
    /// <param name="mediator">Instância do MediatR para processar comandos e queries</param>
    public UsersController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Lista usuários com paginação
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Itens por página (padrão: 20, máximo: 100)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Lista paginada de usuários</returns>
    /// <response code="200">Lista retornada com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não possui permissão SEC.Users.Read</response>
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
    /// Busca usuário por ID
    /// </summary>
    /// <param name="id">ID único do usuário</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Detalhes do usuário</returns>
    /// <response code="200">Usuário encontrado</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpGet("{id:guid}")]
    [Authorize]
    [HasPermission(PermissionConstants.SecUsersRead)]
    [ProducesResponseType(typeof(ApiResponse<UserDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserDetailDto?>>> GetById(Guid id, CancellationToken ct)
        => ApiResponse<UserDetailDto?>.Ok(await _mediator.Send(new RhSensoERP.Application.Security.Users.Queries.GetUserByIdQuery(id), ct));

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    /// <param name="dto">Dados do usuário</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>ID do usuário criado</returns>
    /// <response code="201">Usuário criado com sucesso</response>
    /// <response code="400">Dados inválidos ou usuário já existe</response>
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
    /// Atualiza um usuário existente
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="dto">Dados para atualização</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Confirmação da atualização</returns>
    /// <response code="200">Usuário atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Usuário não encontrado</response>
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
    /// Remove um usuário (exclusão lógica)
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Confirmação da exclusão</returns>
    /// <response code="200">Usuário removido com sucesso</response>
    /// <response code="404">Usuário não encontrado</response>
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