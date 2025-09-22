using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Application.Security.SaaS;

public record ResendConfirmationRequest
{
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
    public string Email { get; init; } = string.Empty;
}