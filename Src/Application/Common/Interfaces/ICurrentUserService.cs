namespace RhSensoERP.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    Guid? TenantId { get; }
}
