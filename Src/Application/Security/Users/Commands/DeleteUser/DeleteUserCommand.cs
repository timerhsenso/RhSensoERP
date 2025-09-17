using MediatR;

namespace RhSensoERP.Application.Security.Users.Commands;

public sealed record DeleteUserCommand(Guid Id) : IRequest<Unit>;
