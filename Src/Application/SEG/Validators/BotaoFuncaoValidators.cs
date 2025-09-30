using FluentValidation;
using RhSensoERP.Application.SEG.DTOs;

namespace RhSensoERP.Application.SEG.Validators
{
    public sealed class BotaoFuncaoDtoValidator : AbstractValidator<BotaoFuncaoDto>
    {
        public BotaoFuncaoDtoValidator()
        {
            RuleFor(x => x.CdSistema).NotEmpty().MaximumLength(10);
            RuleFor(x => x.CdFuncao).NotEmpty().MaximumLength(30);
            RuleFor(x => x.NmBotao).NotEmpty().MaximumLength(30);
            RuleFor(x => x.DcBotao).NotEmpty().MaximumLength(60);

            RuleFor(x => x.CdAcao)
                .Must(c => "IAEC".Contains(char.ToUpperInvariant(c)))
                .WithMessage("CdAcao deve ser um dos valores: I, A, E, C.");
        }
    }
}
