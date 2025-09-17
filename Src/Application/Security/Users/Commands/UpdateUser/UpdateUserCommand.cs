using MediatR;
using RhSensoERP.Application.Security.Users.Dtos;

namespace RhSensoERP.Application.Security.Users.Commands;

public sealed record UpdateUserCommand(Guid Id, UserUpdateDto Dto) : IRequest<Unit>;
