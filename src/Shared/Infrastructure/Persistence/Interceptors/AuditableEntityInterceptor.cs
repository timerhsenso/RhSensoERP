namespace RhSensoERP.Shared.Infrastructure.Persistence.Interceptors;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RhSensoERP.Shared.Core.Abstractions;
using RhSensoERP.Shared.Core.Primitives;

/// <summary>
/// Interceptor que preenche automaticamente campos de auditoria.
/// Verifica se as propriedades existem no modelo antes de tentar modificá-las.
/// </summary>
public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUser _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuditableEntityInterceptor(
        ICurrentUser currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var now = _dateTimeProvider.UtcNow;
        //var userId = _currentUser.UserId ?? "System";
        var userId = _currentUser?.UserId?.ToString() ?? "System";


        // Atualizar entidades BaseEntity (que têm auditoria)
        var baseEntries = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .ToList();

        foreach (var entry in baseEntries)
        {
            // Verificar se as propriedades de auditoria existem no modelo
            var entityType = entry.Metadata;
            var hasCreatedAt = entityType.FindProperty(nameof(BaseEntity.CreatedAt)) != null;
            var hasCreatedBy = entityType.FindProperty(nameof(BaseEntity.CreatedBy)) != null;
            var hasUpdatedAt = entityType.FindProperty(nameof(BaseEntity.UpdatedAt)) != null;
            var hasUpdatedBy = entityType.FindProperty(nameof(BaseEntity.UpdatedBy)) != null;

            if (entry.State == EntityState.Added)
            {
                if (hasCreatedAt)
                {
                    entry.Entity.CreatedAt = now;
                }
                if (hasCreatedBy)
                {
                    entry.Entity.CreatedBy = userId;
                }
            }

            if (entry.State == EntityState.Modified)
            {
                if (hasUpdatedAt)
                {
                    entry.Entity.UpdatedAt = now;
                }
                if (hasUpdatedBy)
                {
                    entry.Entity.UpdatedBy = userId;
                }

                // Prevenir modificação dos campos de criação (somente se existirem)
                if (hasCreatedAt)
                {
                    entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                }
                if (hasCreatedBy)
                {
                    entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
                }
            }
        }

        // Processar soft deletes
        var softDeleteEntries = context.ChangeTracker
            .Entries<ISoftDelete>()
            .Where(e => e.State == EntityState.Deleted)
            .ToList();

        foreach (var entry in softDeleteEntries)
        {
            var entityType = entry.Metadata;
            var hasIsDeleted = entityType.FindProperty(nameof(ISoftDelete.IsDeleted)) != null;
            var hasDeletedAt = entityType.FindProperty(nameof(ISoftDelete.DeletedAt)) != null;
            var hasDeletedBy = entityType.FindProperty(nameof(ISoftDelete.DeletedBy)) != null;

            if (hasIsDeleted || hasDeletedAt || hasDeletedBy)
            {
                entry.State = EntityState.Modified;

                if (hasIsDeleted)
                {
                    entry.Entity.IsDeleted = true;
                }
                if (hasDeletedAt)
                {
                    entry.Entity.DeletedAt = now;
                }
                if (hasDeletedBy)
                {
                    entry.Entity.DeletedBy = userId;
                }
            }
        }
    }
}