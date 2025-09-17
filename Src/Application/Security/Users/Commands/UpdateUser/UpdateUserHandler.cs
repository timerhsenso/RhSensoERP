using MediatR;
using RhSensoERP.Application.Common.Interfaces;
using RhSensoERP.Application.Security.Users.Services;

namespace RhSensoERP.Application.Security.Users.Commands;

public sealed class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Unit>
{
    private readonly IUserService _service;
    private readonly ICurrentUserService _current;
    public UpdateUserHandler(IUserService service, ICurrentUserService current) => (_service, _current) = (service, current);

    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var updatedBy = _current.UserId ?? "api";
        await _service.UpdateAsync(request.Id, request.Dto, updatedBy, cancellationToken);
        return Unit.Value;
    }
}
