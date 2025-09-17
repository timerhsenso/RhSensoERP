using MediatR;
using RhSensoERP.Application.Security.Users.Dtos;
using RhSensoERP.Application.Security.Users.Services;

namespace RhSensoERP.Application.Security.Users.Queries;

public sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDetailDto?>
{
    private readonly IUserService _service;
    public GetUserByIdHandler(IUserService service) => _service = service;

    public Task<UserDetailDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        => _service.GetAsync(request.Id, cancellationToken);
}
