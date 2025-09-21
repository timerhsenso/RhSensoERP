using FluentValidation;
using RhSensoERP.Application.Security.Auth.DTOs;

namespace RhSensoERP.Application.Security.Auth.Validators;

/// <summary>
/// Validador para requisição de login
/// Suporta formatos: "usuario", "DOMAIN\usuario", ou campos separados
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.CdUsuario)
            .NotEmpty().WithMessage("Código do usuário é obrigatório")
            .Length(1, 50).WithMessage("Código do usuário deve ter entre 1 e 50 caracteres")
            .Must(BeValidUserFormat).WithMessage("Formato de usuário inválido. Use 'usuario' ou 'DOMAIN\\usuario'");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(3).WithMessage("Senha deve ter no mínimo 3 caracteres")
            .MaximumLength(50).WithMessage("Senha deve ter no máximo 50 caracteres");

        // Validação opcional do domínio se fornecido separadamente
        RuleFor(x => x.Domain)
            .MaximumLength(50).WithMessage("Domínio deve ter no máximo 50 caracteres")
            .Matches(@"^[a-zA-Z0-9._-]+$").WithMessage("Domínio contém caracteres inválidos")
            .When(x => !string.IsNullOrWhiteSpace(x.Domain));
    }

    /// <summary>
    /// Valida se o formato do usuário é aceitável
    /// Aceita: "usuario", "DOMAIN\usuario", "usuario@domain.com"
    /// </summary>
    private static bool BeValidUserFormat(string? usuario)
    {
        if (string.IsNullOrWhiteSpace(usuario))
            return false;

        // Se contém barra invertida, deve ser formato DOMAIN\user válido
        if (usuario.Contains('\\'))
        {
            var parts = usuario.Split('\\', 2);
            if (parts.Length != 2 ||
                string.IsNullOrWhiteSpace(parts[0]) ||
                string.IsNullOrWhiteSpace(parts[1]))
            {
                return false;
            }

            // Valida cada parte separadamente
            return IsValidUsernamePart(parts[0]) && IsValidUsernamePart(parts[1]);
        }

        // Formato simples ou email
        return IsValidUsernamePart(usuario) || IsValidEmail(usuario);
    }

    /// <summary>
    /// Valida parte individual do nome de usuário
    /// </summary>
    private static bool IsValidUsernamePart(string part)
    {
        if (string.IsNullOrWhiteSpace(part) || part.Length > 30)
            return false;

        // Permite: letras, números, ponto, underscore, hífen
        return part.All(c => char.IsLetterOrDigit(c) || c == '.' || c == '_' || c == '-');
    }

    /// <summary>
    /// Validação básica de email para modo SaaS
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        return email.Contains('@') && email.Contains('.') && email.Length <= 100;
    }
}