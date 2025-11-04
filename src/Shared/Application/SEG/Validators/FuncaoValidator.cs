// RhSensoERP.Application/Validators/FuncaoValidator.cs
using FluentValidation;
//using RhSensoERP.Application.DTOs.Funcao;
using RhSensoERP.Application.SEG.DTOs;
namespace RhSensoERP.Shared.Application.Validators;

public class CreateFuncaoDTOValidator : AbstractValidator<CreateFuncaoDTO>
{
    public CreateFuncaoDTOValidator()
    {
        RuleFor(x => x.CdFuncao)
            .NotEmpty().WithMessage("Código da função é obrigatório")
            .MaximumLength(30).WithMessage("Código da função deve ter no máximo 30 caracteres");

        RuleFor(x => x.CdSistema)
            .NotEmpty().WithMessage("Código do sistema é obrigatório")
            .MaximumLength(10).WithMessage("Código do sistema deve ter no máximo 10 caracteres");

        RuleFor(x => x.DcFuncao)
            .MaximumLength(80).WithMessage("Descrição da função deve ter no máximo 80 caracteres");

        RuleFor(x => x.DcModulo)
            .MaximumLength(100).WithMessage("Descrição do módulo deve ter no máximo 100 caracteres");

        RuleFor(x => x.DescricaoModulo)
            .MaximumLength(100).WithMessage("Descrição do módulo deve ter no máximo 100 caracteres");
    }
}