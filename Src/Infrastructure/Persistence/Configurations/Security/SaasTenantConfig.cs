using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.Security;

/// <summary>
/// Configuração EF Core para entidade SaasTenant
/// </summary>
public class SaasTenantConfig : IEntityTypeConfiguration<SaasTenant>
{
    public void Configure(EntityTypeBuilder<SaasTenant> b)
    {
        b.ToTable("SaasTenants", schema: "dbo");

        // Chave primária
        b.HasKey(x => x.Id);

        // Campos obrigatórios
        b.Property(x => x.CompanyName)
            .HasColumnName("CompanyName")
            .HasColumnType("nvarchar(255)")
            .HasMaxLength(255)
            .IsRequired();

        b.Property(x => x.Domain)
            .HasColumnName("Domain")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100)
            .IsRequired(false);

        b.Property(x => x.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .IsRequired()
            .HasDefaultValue(true);

        // Configurações de plano
        b.Property(x => x.MaxUsers)
            .HasColumnName("MaxUsers")
            .HasColumnType("int")
            .IsRequired()
            .HasDefaultValue(10);

        b.Property(x => x.PlanType)
            .HasColumnName("PlanType")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Basic");

        // Auditoria
        b.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        b.Property(x => x.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        b.Property(x => x.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100)
            .IsRequired(false);

        b.Property(x => x.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100)
            .IsRequired(false);

        // Ignorar propriedades de conveniência
        b.Ignore(x => x.ActiveUsersCount);
        b.Ignore(x => x.CanAddMoreUsers);
        b.Ignore(x => x.AvailableUserSlots);

        // Índices únicos
        b.HasIndex(x => x.Domain)
            .IsUnique()
            .HasDatabaseName("UK_SaasTenants_Domain")
            .HasFilter("[Domain] IS NOT NULL");

        b.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_SaasTenants_IsActive");

        // Relacionamentos
        b.HasMany(x => x.Users)
            .WithOne(x => x.Tenant)
            .HasForeignKey(x => x.TenantId)
            .HasConstraintName("FK_SaasUsers_SaasTenants_TenantId")
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Invitations)
            .WithOne(x => x.Tenant)
            .HasForeignKey(x => x.TenantId)
            .HasConstraintName("FK_SaasInvitations_SaasTenants_TenantId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
