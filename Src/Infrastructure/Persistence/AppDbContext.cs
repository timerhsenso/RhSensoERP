using Microsoft.EntityFrameworkCore;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Infrastructure.Persistence.Configurations.Security;
using RhSensoERP.Infrastructure.Persistence.Interceptors;

namespace RhSensoERP.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    private readonly AuditSaveChangesInterceptor _auditInterceptor;

    public AppDbContext(DbContextOptions<AppDbContext> options, AuditSaveChangesInterceptor auditInterceptor) : base(options)
        => _auditInterceptor = auditInterceptor;

    public DbSet<User> Users => Set<User>();
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfig());

        // REMOVER o global query filter que estava causando erro
        // O sistema legacy não usa soft delete, usa FlAtivo = 'S'/'N'
    }
}