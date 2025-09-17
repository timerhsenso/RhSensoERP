using RhSensoERP.Application.Security.Users.Dtos;
using RhSensoERP.Core.Abstractions.Paging;

namespace RhSensoERP.Application.Security.Users.Services;

public interface IUserService
{
    Task<UserDetailDto?> GetAsync(Guid id, CancellationToken ct);
    Task<PagedResult<UserListDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
    Task<Guid> CreateAsync(UserCreateDto dto, Guid tenantId, string createdBy, CancellationToken ct);
    Task UpdateAsync(Guid id, UserUpdateDto dto, string updatedBy, CancellationToken ct);
    Task DeleteAsync(Guid id, string deletedBy, CancellationToken ct);
}
