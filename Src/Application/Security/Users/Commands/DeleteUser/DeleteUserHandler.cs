using MediatR;
using RhSensoERP.Application.Common.Interfaces;
using RhSensoERP.Application.Security.Users.Services;

namespace RhSensoERP.Application.Security.Users.Commands;

public sealed class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IUserService _service;
    private readonly ICurrentUserService _current;
    public DeleteUserHandler(IUserService service, ICurrentUserService current) => (_service, _current) = (service, current);

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var deletedBy = _current.UserId ?? "api";
        await _service.DeleteAsync(request.Id, deletedBy, cancellationToken);
        return Unit.Value;
    }
}
