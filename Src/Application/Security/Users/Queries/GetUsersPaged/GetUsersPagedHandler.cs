using MediatR;
using RhSensoERP.Application.Security.Users.Dtos;
using RhSensoERP.Application.Security.Users.Services;
using RhSensoERP.Core.Abstractions.Paging;

namespace RhSensoERP.Application.Security.Users.Queries;

public sealed class GetUsersPagedHandler : IRequestHandler<GetUsersPagedQuery, PagedResult<UserListDto>>
{
    private readonly IUserService _service;
    public GetUsersPagedHandler(IUserService service) => _service = service;

    public Task<PagedResult<UserListDto>> Handle(GetUsersPagedQuery request, CancellationToken cancellationToken)
        => _service.GetPagedAsync(request.Page, request.PageSize, cancellationToken);
}
