using MediatR;
using RhSensoERP.Application.Security.Users.Dtos;

namespace RhSensoERP.Application.Security.Users.Commands;

public sealed record CreateUserCommand(UserCreateDto Dto) : IRequest<Guid>;
