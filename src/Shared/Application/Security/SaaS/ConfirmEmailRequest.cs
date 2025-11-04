using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Shared.Application.Security.SaaS;

public record ConfirmEmailRequest
{
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Token de confirmação é obrigatório")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Token deve ter entre 10 e 100 caracteres")]
    public string Token { get; init; } = string.Empty;
}