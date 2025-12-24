// ============================================================================
// ARQUIVO ALTERADO - SUBSTITUIR: src/Identity/Application/Validators/Auth/LoginRequestValidator.cs
// ============================================================================

using FluentValidation;
using RhSensoERP.Identity.Application.DTOs.Auth;

namespace RhSensoERP.Identity.Application.Validators.Auth;

/// <summary>
/// Validador do request de login.
/// </summary>
public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        // ✅ ALTERADO - FASE 1: Validação de LoginIdentifier ao invés de CdUsuario
        RuleFor(x => x.LoginIdentifier)
            .NotEmpty()
            .WithMessage("Identificador de login é obrigatório.")
            .MaximumLength(100)
            .WithMessage("Identificador de login deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Senha)
            .NotEmpty()
            .WithMessage("Senha é obrigatória.")
            .MinimumLength(1)
            .WithMessage("Senha não pode ser vazia.")
            .MaximumLength(100)
            .WithMessage("Senha deve ter no máximo 100 caracteres.");
    }
}
