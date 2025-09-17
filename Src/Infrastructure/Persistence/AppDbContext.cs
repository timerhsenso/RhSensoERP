using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Core.Abstractions.Entities;
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfig());

        // Global query filters for soft-delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var prop = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var compare = Expression.Equal(prop, Expression.Constant(false));
                var lambda = Expression.Lambda(compare, parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}
