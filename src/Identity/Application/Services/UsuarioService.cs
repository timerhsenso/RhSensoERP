using AutoMapper;
using RhSensoERP.Identity.Application.DTOs.Usuario;
using RhSensoERP.Identity.Infrastructure.Repositories;

namespace RhSensoERP.Identity.Application.Services;

public interface IUsuarioService
{
    Task<UsuarioDto?> GetAsync(string cdUsuario, CancellationToken ct);
    Task<List<UsuarioDto>> SearchAsync(string? term, int take, CancellationToken ct);
}

public sealed class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repo;
    private readonly IMapper _mapper;

    public UsuarioService(IUsuarioRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<UsuarioDto?> GetAsync(string cdUsuario, CancellationToken ct)
    {
        var entity = await _repo.GetByCdUsuarioAsync(cdUsuario, ct);
        return entity is null ? null : _mapper.Map<UsuarioDto>(entity);
    }

    public async Task<List<UsuarioDto>> SearchAsync(string? term, int take, CancellationToken ct)
    {
        var list = await _repo.SearchAsync(term, take, ct);
        return _mapper.Map<List<UsuarioDto>>(list);
    }
}
