using AutoMapper;
using RhSensoERP.Application.SEG.DTOs;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Core.SEG.Entities;

namespace RhSensoERP.Application.SEG.Mapping
{
    /// <summary>
    /// Mapeamentos entre Entidade e DTOs de Sistema.
    /// </summary>
    public sealed class SistemaProfile : Profile
    {
        public SistemaProfile()
        {
            // Entidade -> DTO
            CreateMap<Sistema, SistemaDto>();

            // Upsert DTO -> Entidade
            CreateMap<SistemaUpsertDto, Sistema>()
                .ForMember(d => d.CdSistema, opt => opt.MapFrom(s => s.CdSistema))
                .ForMember(d => d.DcSistema, opt => opt.MapFrom(s => s.DcSistema))
                .ForMember(d => d.Ativo, opt => opt.MapFrom(s => s.Ativo));

            // DTO -> Entidade (se precisar em cenários de patch/map inverso)
            CreateMap<SistemaDto, Sistema>();
        }
    }
}
