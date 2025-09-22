using Microsoft.EntityFrameworkCore;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Core.Security.SaaS;
using RhSensoERP.Infrastructure.Persistence;

namespace RhSensoERP.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de usuários SaaS
/// Segue padrão Repository com Entity Framework Core
/// </summary>
public class SaasUserRepository : ISaasUserRepository
{
    private readonly AppDbContext _context;

    public SaasUserRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Busca usuário por email
    /// </summary>
    public async Task<SaasUser?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var normalizedEmail = email.Trim().ToUpperInvariant();

        return await _context.Set<SaasUser>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.EmailNormalized == normalizedEmail);
    }

    /// <summary>
    /// Busca usuário por refresh token
    /// </summary>
    public async Task<SaasUser?> GetByRefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return null;

        return await _context.Set<SaasUser>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken &&
                                     u.RefreshTokenExpiresAt > DateTime.UtcNow);
    }

    /// <summary>
    /// Busca usuário por ID
    /// </summary>
    public async Task<SaasUser?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _context.Set<SaasUser>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// Busca usuário por token de confirmação de email
    /// </summary>
    public async Task<SaasUser?> GetByEmailConfirmationTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        return await _context.Set<SaasUser>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.EmailConfirmationToken == token);
    }

    /// <summary>
    /// Busca usuário por token de reset de senha
    /// </summary>
    public async Task<SaasUser?> GetByPasswordResetTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        return await _context.Set<SaasUser>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.PasswordResetToken == token &&
                                     u.PasswordResetTokenExpiry > DateTime.UtcNow);
    }

    /// <summary>
    /// Adiciona novo usuário
    /// </summary>
    public async Task AddAsync(SaasUser user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        await _context.Set<SaasUser>().AddAsync(user);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Atualiza usuário existente
    /// </summary>
    public async Task UpdateAsync(SaasUser user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        _context.Set<SaasUser>().Update(user);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Remove usuário (soft delete)
    /// </summary>
    public async Task DeleteAsync(SaasUser user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        user.Deactivate(); // Desativa em vez de deletar
        _context.Set<SaasUser>().Update(user);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Verifica se email já existe no tenant
    /// </summary>
    public async Task<bool> EmailExistsInTenantAsync(string email, Guid tenantId)
    {
        if (string.IsNullOrWhiteSpace(email) || tenantId == Guid.Empty)
            return false;

        var normalizedEmail = email.Trim().ToUpperInvariant();

        return await _context.Set<SaasUser>()
            .AsNoTracking()
            .AnyAsync(u => u.EmailNormalized == normalizedEmail && u.TenantId == tenantId);
    }

    /// <summary>
    /// Lista usuários do tenant com paginação
    /// </summary>
    public async Task<(List<SaasUser> Users, int TotalCount)> GetByTenantAsync(Guid tenantId, int page = 1, int pageSize = 20)
    {
        if (tenantId == Guid.Empty)
            return (new List<SaasUser>(), 0);

        var query = _context.Set<SaasUser>()
            .AsNoTracking()
            .Where(u => u.TenantId == tenantId)
            .OrderBy(u => u.FullName);

        var totalCount = await query.CountAsync();

        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (users, totalCount);
    }

    /// <summary>
    /// Conta usuários ativos do tenant
    /// </summary>
    public async Task<int> CountActiveUsersByTenantAsync(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
            return 0;

        return await _context.Set<SaasUser>()
            .AsNoTracking()
            .CountAsync(u => u.TenantId == tenantId && u.IsActive);
    }
}