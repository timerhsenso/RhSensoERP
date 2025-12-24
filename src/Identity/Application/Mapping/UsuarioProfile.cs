using AutoMapper;
using RhSensoERP.Identity.Application.DTOs.Usuario;
using RhSensoERP.Identity.Core.Entities;

namespace RhSensoERP.Identity.Application.Mapping;

public sealed class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        CreateMap<Usuario, UsuarioDto>()
            .ForMember(d => d.Email, m => m.MapFrom(s => s.Email_Usuario));
    }
}
