using AutoMapper;
using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Identity.Core.Entities;

namespace RhSensoERP.Identity.Application.Mapping;

/// <summary>
/// Profile AutoMapper para Auth.
/// </summary>
public sealed class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<Usuario, UserInfoDto>()
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email_Usuario))
            .ForMember(d => d.TenantId, opt => opt.MapFrom(s => s.TenantId))
            .ForMember(d => d.TwoFactorEnabled, opt => opt.Ignore())
            .ForMember(d => d.MustChangePassword, opt => opt.Ignore());
    }
}