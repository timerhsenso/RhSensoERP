using AutoMapper;
using RhSensoERP.Identity.Application.DTOs.Permissoes;
using RhSensoERP.Identity.Core.Entities;

namespace RhSensoERP.Identity.Application.Mapping;

public sealed class PermissaoProfile : Profile
{
    public PermissaoProfile()
    {
        CreateMap<BotaoFuncao, BotaoDto>();
        // FuncaoPermissaoDto será montado por consulta (join) no serviço
    }
}
