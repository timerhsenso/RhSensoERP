using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.Security;

/// <summary>
/// Configuração EF Core para entidade SaasInvitation
/// </summary>
public class SaasInvitationConfig : IEntityTypeConfiguration<SaasInvitation>
{
    public void Configure(EntityTypeBuilder<SaasInvitation> b)
    {
        b.ToTable("SaasInvitations", schema: "dbo");

        // Chave primária
        b.HasKey(x => x.Id);

        // Campos obrigatórios
        b.Property(x => x.Email)
            .HasColumnName("Email")
            .HasColumnType("nvarchar(255)")
            .HasMaxLength(255)
            .IsRequired();

        b.Property(x => x.EmailNormalized)
            .HasColumnName("EmailNormalized")
            .HasColumnType("nvarchar(255)")
            .HasMaxLength(255)
            .IsRequired()
            .HasComputedColumnSql("UPPER([Email])", stored: true);

        b.Property(x => x.TenantId)
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        b.Property(x => x.InvitedById)
            .HasColumnName("InvitedById")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        // Token e controle
        b.Property(x => x.InvitationToken)
            .HasColumnName("InvitationToken")
            .HasColumnType("nvarchar(255)")
            .HasMaxLength(255)
            .IsRequired();

        b.Property(x => x.ExpiresAt)
            .HasColumnName("ExpiresAt")
            .HasColumnType("datetime2")
            .IsRequired();

        b.Property(x => x.AcceptedAt)
            .HasColumnName("AcceptedAt")
            .HasColumnType("datetime2")
            .IsRequired(false);

        b.Property(x => x.IsAccepted)
            .HasColumnName("IsAccepted")
            .HasColumnType("bit")
            .IsRequired()
            .HasDefaultValue(false);

        // Metadados
        b.Property(x => x.Role)
            .HasColumnName("Role")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("User");

        b.Property(x => x.Message)
            .HasColumnName("Message")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500)
            .IsRequired(false);

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

        // Ignorar propriedades de conveniência
        b.Ignore(x => x.IsExpired);
        b.Ignore(x => x.IsValid);
        b.Ignore(x => x.TimeUntilExpiry);

        // Índices únicos
        b.HasIndex(x => new { x.EmailNormalized, x.TenantId })
            .IsUnique()
            .HasDatabaseName("UK_SaasInvitations_EmailNormalized_TenantId")
            .HasFilter("[IsAccepted] = 0");

        b.HasIndex(x => x.InvitationToken)
            .IsUnique()
            .HasDatabaseName("IX_SaasInvitations_InvitationToken");

        b.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName("IX_SaasInvitations_ExpiresAt");

        // Relacionamentos
        b.HasOne(x => x.Tenant)
            .WithMany(x => x.Invitations)
            .HasForeignKey(x => x.TenantId)
            .HasConstraintName("FK_SaasInvitations_SaasTenants_TenantId")
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.InvitedBy)
            .WithMany(x => x.SentInvitations)
            .HasForeignKey(x => x.InvitedById)
            .HasConstraintName("FK_SaasInvitations_SaasUsers_InvitedById")
            .OnDelete(DeleteBehavior.Restrict);
    }
}