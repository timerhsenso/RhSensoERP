using MediatR;
using RhSensoERP.Application.Common.Interfaces;
using RhSensoERP.Application.Security.Users.Dtos;
using RhSensoERP.Application.Security.Users.Services;

namespace RhSensoERP.Application.Security.Users.Commands;

public sealed class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserService _service;
    private readonly ICurrentUserService _current;
    public CreateUserHandler(IUserService service, ICurrentUserService current) => (_service, _current) = (service, current);

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _current.TenantId ?? throw new InvalidOperationException("Tenant not found");
        var createdBy = _current.UserId ?? "api";
        return await _service.CreateAsync(request.Dto, tenantId, createdBy, cancellationToken);
    }
}
