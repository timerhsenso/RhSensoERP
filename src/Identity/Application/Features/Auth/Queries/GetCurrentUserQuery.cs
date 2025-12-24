using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Identity.Infrastructure.Persistence.Contexts;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Identity.Application.Features.Auth.Queries;

/// <summary>
/// Query para obter dados do usuário atual.
/// </summary>
public sealed record GetCurrentUserQuery(string CdUsuario) : IRequest<Result<UserInfoDto>>;

/// <summary>
/// Handler do GetCurrentUserQuery.
/// </summary>
public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserInfoDto>>
{
    private readonly IdentityDbContext _db;
    private readonly IMapper _mapper;

    public GetCurrentUserQueryHandler(IdentityDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<UserInfoDto>> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        var usuario = await _db.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.CdUsuario == request.CdUsuario, ct);

        if (usuario == null)
        {
            return Result<UserInfoDto>.Failure("USER_NOT_FOUND", "Usuário não encontrado.");
        }

        var userInfo = _mapper.Map<UserInfoDto>(usuario);

        // Buscar dados adicionais do UserSecurity se existir
        var userSecurity = await _db.Set<Core.Entities.UserSecurity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(us => us.IdUsuario == usuario.Id, ct);

        if (userSecurity != null)
        {
            userInfo = userInfo with
            {
                TwoFactorEnabled = userSecurity.TwoFactorEnabled,
                MustChangePassword = userSecurity.MustChangePassword
            };
        }

        return Result<UserInfoDto>.Success(userInfo);
    }
}