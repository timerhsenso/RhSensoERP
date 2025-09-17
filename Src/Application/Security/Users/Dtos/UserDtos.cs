namespace RhSensoERP.Application.Security.Users.Dtos;

public sealed record UserListDto(Guid Id, string Username, string DisplayName, string Email, bool Active);
public sealed record UserDetailDto(Guid Id, string Username, string DisplayName, string Email, bool Active, Guid TenantId);

public sealed class UserCreateDto
{
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class UserUpdateDto
{
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Active { get; set; }
}
