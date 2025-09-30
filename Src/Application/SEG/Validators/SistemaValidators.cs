using FluentValidation;
using RhSensoERP.Application.SEG.DTOs;

namespace RhSensoERP.Application.SEG.Validation
{
    public sealed class SistemaUpsertValidator : AbstractValidator<SistemaUpsertDto>
    {
        public SistemaUpsertValidator()
        {
            RuleFor(x => x.CdSistema)
                .NotEmpty().WithMessage("Código do sistema é obrigatório.")
                .MaximumLength(10).WithMessage("Código do sistema deve ter no máximo 10 caracteres.");

            RuleFor(x => x.DcSistema)
                .NotEmpty().WithMessage("Descrição do sistema é obrigatória.")
                .MaximumLength(60).WithMessage("Descrição do sistema deve ter no máximo 60 caracteres.");

            // Ativo é bool (default true) — nenhuma regra adicional necessária
        }
    }
}
