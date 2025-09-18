using FluentValidation;
using RhSensoERP.Application.Security.Auth.DTOs;

namespace RhSensoERP.Application.Security.Auth.Validators;

/// <summary>
/// Validador para requisição de login
/// Verifica se dados de entrada estão em formato correto
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.CdUsuario)
            .NotEmpty().WithMessage("Código do usuário é obrigatório")
            .Length(1, 30).WithMessage("Código do usuário deve ter entre 1 e 30 caracteres")
            .Matches(@"^[a-zA-Z0-9._-]+$").WithMessage("Código do usuário contém caracteres inválidos");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(3).WithMessage("Senha deve ter no mínimo 4 caracteres")
            .MaximumLength(50).WithMessage("Senha deve ter no máximo 50 caracteres");
    }
}