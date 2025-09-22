namespace RhSensoERP.Application.Security.SaaS;

/// <summary>
/// Response do registro SaaS
/// </summary>
public record SaasRegistrationResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required Guid TenantId { get; init; }
    public required DateTime CreatedAt { get; init; }
    public bool RequiresEmailConfirmation { get; init; } = true;
    public bool EmailConfirmationSent { get; init; }
    public string Message { get; init; } = string.Empty;
}
