using FluentValidation;

namespace RhSensoERP.Application.Security.Users.Commands;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Dto.Username).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(x => x.Dto.DisplayName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Dto.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Dto.Password).NotEmpty().MinimumLength(8);
    }
}
