using MediatR;
using RhSensoERP.Application.Security.Users.Dtos;
using RhSensoERP.Core.Abstractions.Paging;

namespace RhSensoERP.Application.Security.Users.Queries;

public sealed record GetUsersPagedQuery(int Page = 1, int PageSize = 20) : IRequest<PagedResult<UserListDto>>;
