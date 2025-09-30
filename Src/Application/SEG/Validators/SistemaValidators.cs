using FluentValidation;
using RhSensoERP.Application.SEG.DTOs;

namespace RhSensoERP.Application.SEG.Validators
{
    public sealed class SistemaValidator : AbstractValidator<SistemaUpsertDto>
    {
        public SistemaValidator()
        {
            RuleFor(x => x.CdSistema)
                .NotEmpty().WithMessage("C�digo do sistema � obrigat�rio.")
                .MaximumLength(10).WithMessage("C�digo do sistema deve ter no m�ximo 10 caracteres.");

            RuleFor(x => x.DcSistema)
                .NotEmpty().WithMessage("Descri��o do sistema � obrigat�ria.")
                .MaximumLength(60).WithMessage("Descri��o do sistema deve ter no m�ximo 60 caracteres.");
        }
    }
}
