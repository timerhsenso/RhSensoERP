using MediatR;
using RhSensoERP.Application.Security.Users.Dtos;

namespace RhSensoERP.Application.Security.Users.Queries;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserDetailDto?>;
