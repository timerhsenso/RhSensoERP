namespace RhSensoERP.Shared.Application.Security.SaaS;

public record ConfirmEmailResponse
{
    public bool EmailConfirmed { get; init; }
    public string Message { get; init; } = string.Empty;
    public DateTime ConfirmedAt { get; init; }
}