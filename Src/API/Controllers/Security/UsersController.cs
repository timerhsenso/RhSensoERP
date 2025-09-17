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

[ApiController]
[Route("api/v1/security/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize]
    [HasPermission(PermissionConstants.SecUsersRead)]
    public async Task<ActionResult<ApiResponse<PagedResult<UserListDto>>>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => ApiResponse<PagedResult<UserListDto>>.Ok(await _mediator.Send(new RhSensoERP.Application.Security.Users.Queries.GetUsersPagedQuery(page, Math.Min(pageSize, 100)), ct));

    [HttpGet("{id:guid}")]
    [Authorize]
    [HasPermission(PermissionConstants.SecUsersRead)]
    public async Task<ActionResult<ApiResponse<UserDetailDto?>>> GetById(Guid id, CancellationToken ct)
        => ApiResponse<UserDetailDto?>.Ok(await _mediator.Send(new RhSensoERP.Application.Security.Users.Queries.GetUserByIdQuery(id), ct));

    [HttpPost]
    [Authorize]
    [HasPermission(PermissionConstants.SecUsersWrite)]
    public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] UserCreateDto dto, CancellationToken ct)
    {
        var id = await _mediator.Send(new RhSensoERP.Application.Security.Users.Commands.CreateUserCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id }, ApiResponse<object>.Ok(new { id }));
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    [HasPermission(PermissionConstants.SecUsersWrite)]
    public async Task<ActionResult<ApiResponse<object>>> Update(Guid id, [FromBody] UserUpdateDto dto, CancellationToken ct)
    {
        await _mediator.Send(new RhSensoERP.Application.Security.Users.Commands.UpdateUserCommand(id, dto), ct);
        return ApiResponse<object>.Ok(new { id });
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    [HasPermission(PermissionConstants.SecUsersDelete)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new RhSensoERP.Application.Security.Users.Commands.DeleteUserCommand(id), ct);
        return ApiResponse<object>.Ok(new { id });
    }
}
