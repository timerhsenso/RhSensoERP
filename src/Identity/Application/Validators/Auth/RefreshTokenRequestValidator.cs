using FluentValidation;
using RhSensoERP.Identity.Application.DTOs.Auth;

namespace RhSensoERP.Identity.Application.Validators.Auth;

/// <summary>
/// Validador do request de refresh token.
/// </summary>
public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token é obrigatório.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token é obrigatório.")
            .Length(32, 500).WithMessage("Refresh token inválido.");
    }
}