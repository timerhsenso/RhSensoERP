using FluentValidation;
using RhSensoERP.Application.Security.Auth.DTOs;

namespace RhSensoERP.Shared.Application.Security.Auth.Validators;

/// <summary>
/// Validador para requisição de login com FluentValidation
/// Suporta múltiplos formatos:
/// - Usuário simples: "usuario"
/// - Formato domínio: "DOMAIN\usuario" (raro, legado)
/// - Email corporativo: "usuario@empresa.com.br" (modo SaaS)
/// 
/// IMPORTANTE: O domínio AD normalmente vem da configuração do backend,
/// não do frontend. Este validador suporta ambos os cenários.
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    // Constantes para configuração centralizada
    private const int MinUsernameLength = 1;
    private const int MaxUsernameLength = 100;
    private const int MinPasswordLength = 3;
    private const int MaxPasswordLength = 100;
    private const int MaxDomainLength = 50;

    public LoginRequestValidator()
    {
        // Validação: Código do Usuário
        RuleFor(x => x.CdUsuario)
            .NotEmpty()
            .WithMessage("Código do usuário é obrigatório")
            .Length(MinUsernameLength, MaxUsernameLength)
            .WithMessage($"Código do usuário deve ter entre {MinUsernameLength} e {MaxUsernameLength} caracteres")
            .Must(BeValidUserFormat)
            .WithMessage("Formato de usuário inválido. Use: 'usuario', 'DOMAIN\\usuario' ou 'usuario@dominio.com'");

        // Validação: Senha
        RuleFor(x => x.Senha)
            .NotEmpty()
            .WithMessage("Senha é obrigatória")
            .Length(MinPasswordLength, MaxPasswordLength)
            .WithMessage($"Senha deve ter entre {MinPasswordLength} e {MaxPasswordLength} caracteres");

        // Validação: Domínio (opcional - geralmente vem da config do servidor)
        RuleFor(x => x.Domain)
            .MaximumLength(MaxDomainLength)
            .WithMessage($"Domínio deve ter no máximo {MaxDomainLength} caracteres")
            .Matches(@"^[a-zA-Z0-9._-]+$")
            .WithMessage("Domínio contém caracteres inválidos. Use apenas letras, números, ponto, hífen e underscore")
            .When(x => !string.IsNullOrWhiteSpace(x.Domain));
    }

    /// <summary>
    /// Valida se o formato do usuário é aceitável para login
    /// Suporta múltiplos formatos corporativos:
    /// - Simples: "jsilva" ou "joao.silva"
    /// - Domínio: "EMPRESA\jsilva"
    /// - Email: "jsilva@empresa.com.br"
    /// </summary>
    /// <param name="usuario">Nome de usuário a validar</param>
    /// <returns>true se formato válido, false caso contrário</returns>
    private static bool BeValidUserFormat(string? usuario)
    {
        if (string.IsNullOrWhiteSpace(usuario))
            return false;

        // Formato 1: DOMAIN\usuario (Active Directory / LDAP)
        if (usuario.Contains('\\'))
        {
            return IsValidDomainUserFormat(usuario);
        }

        // Formato 2: Email corporativo (modo SaaS)
        if (usuario.Contains('@'))
        {
            return IsValidEmail(usuario);
        }

        // Formato 3: Usuário simples (banco local)
        return IsValidUsernamePart(usuario);
    }

    /// <summary>
    /// Valida formato DOMAIN\usuario para autenticação Windows/AD
    /// </summary>
    private static bool IsValidDomainUserFormat(string usuario)
    {
        var parts = usuario.Split('\\', 2);

        // Deve ter exatamente 2 partes (domínio e usuário)
        if (parts.Length != 2)
            return false;

        var domain = parts[0];
        var username = parts[1];

        // Ambas as partes devem ser válidas
        return !string.IsNullOrWhiteSpace(domain) &&
               !string.IsNullOrWhiteSpace(username) &&
               IsValidUsernamePart(domain) &&
               IsValidUsernamePart(username);
    }

    /// <summary>
    /// Valida parte individual do nome de usuário
    /// Aceita: letras, números, ponto, underscore, hífen
    /// </summary>
    private static bool IsValidUsernamePart(string part)
    {
        if (string.IsNullOrWhiteSpace(part))
            return false;

        if (part.Length > MaxUsernameLength)
            return false;

        // Caracteres permitidos em username corporativo
        // Evita espaços, caracteres especiais que possam causar injection
        return part.All(c =>
            char.IsLetterOrDigit(c) ||
            c == '.' ||
            c == '_' ||
            c == '-');
    }

    /// <summary>
    /// Validação básica de formato de email
    /// Para modo SaaS onde login = email corporativo
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        if (email.Length > MaxUsernameLength)
            return false;

        // Validação básica de estrutura de email
        var atIndex = email.IndexOf('@');
        var dotIndex = email.LastIndexOf('.');

        return atIndex > 0 &&                    // @ não pode ser o primeiro caractere
               dotIndex > atIndex &&              // deve ter . após @
               dotIndex < email.Length - 1 &&     // . não pode ser o último caractere
               email.Count(c => c == '@') == 1;   // apenas um @
    }
}