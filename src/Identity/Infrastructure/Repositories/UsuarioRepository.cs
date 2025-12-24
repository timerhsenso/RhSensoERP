using Microsoft.EntityFrameworkCore;
using RhSensoERP.Identity.Core.Entities;
using RhSensoERP.Identity.Infrastructure.Persistence.Contexts;

namespace RhSensoERP.Identity.Infrastructure.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByCdUsuarioAsync(string cdUsuario, CancellationToken ct);
    Task<List<Usuario>> SearchAsync(string? term, int take, CancellationToken ct);
    Task<bool> ExistsAsync(string cdUsuario, CancellationToken ct);
}

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly IdentityDbContext _db;

    public UsuarioRepository(IdentityDbContext db) => _db = db;

    public Task<Usuario?> GetByCdUsuarioAsync(string cdUsuario, CancellationToken ct) =>
        _db.Usuarios
           .AsNoTracking()
           .FirstOrDefaultAsync(u => u.CdUsuario == cdUsuario, ct);

    public async Task<List<Usuario>> SearchAsync(string? term, int take, CancellationToken ct)
    {
        var q = _db.Usuarios.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(term))
        {
            term = term.Trim();
            q = q.Where(u => u.CdUsuario.Contains(term) ||
                             u.DcUsuario.Contains(term) ||
                             (u.Email_Usuario != null && u.Email_Usuario.Contains(term)));
        }

        return await q.OrderBy(u => u.CdUsuario)
                      .Take(take <= 0 ? 20 : take)
                      .ToListAsync(ct);
    }

    public Task<bool> ExistsAsync(string cdUsuario, CancellationToken ct) =>
        _db.Usuarios.AsNoTracking().AnyAsync(u => u.CdUsuario == cdUsuario, ct);
}
