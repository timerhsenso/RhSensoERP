using System.Security.Cryptography;
using AutoMapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using RhSensoERP.Application.Common.Interfaces;
using RhSensoERP.Application.Security.Users.Dtos;
using RhSensoERP.Core.Abstractions.Interfaces;
using RhSensoERP.Core.Abstractions.Paging;
using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Application.Security.Users.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _repo;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _current;
    private readonly IMapper _mapper;

    public UserService(IRepository<User> repo, IUnitOfWork uow, ICurrentUserService current, IMapper mapper)
        => (_repo, _uow, _current, _mapper) = (repo, uow, current, mapper);

    public async Task<UserDetailDto?> GetAsync(Guid id, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(id, ct);
        return user is null ? null : _mapper.Map<UserDetailDto>(user);
    }

    public async Task<PagedResult<UserListDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        var result = await _repo.GetPagedAsync(page, pageSize, ct);
        return new PagedResult<UserListDto>
        {
            Items = _mapper.Map<List<UserListDto>>(result.Items),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<Guid> CreateAsync(UserCreateDto dto, Guid tenantId, string createdBy, CancellationToken ct)
    {
        var user = _mapper.Map<User>(dto);
        user.TenantId = tenantId;
        user.PasswordHash = HashPassword(dto.Password);
        
        await _repo.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);
        return user.Id;
    }

    public async Task UpdateAsync(Guid id, UserUpdateDto dto, string updatedBy, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("User not found");
        
        // Mapeia apenas as propriedades permitidas para atualização
        _mapper.Map(dto, user);
        
        await _repo.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, string deletedBy, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("User not found");
        await _repo.DeleteAsync(user, ct);
        await _uow.SaveChangesAsync(ct);
    }

    private static string HashPassword(string password)
    {
        // Simple PBKDF2 sample (replace with a stronger implementation or Identity later)
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 100_000, 32);
        return Convert.ToBase64String(salt.Concat(hash).ToArray());
    }
}
