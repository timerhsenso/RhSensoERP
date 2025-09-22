using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.Security;

/// <summary>
/// Configuração EF Core para entidade SaasUser
/// </summary>
public class SaasUserConfig : IEntityTypeConfiguration<SaasUser>
{
    public void Configure(EntityTypeBuilder<SaasUser> b)
    {
        b.ToTable("SaasUsers", schema: "dbo");

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

        b.Property(x => x.PasswordHash)
            .HasColumnName("PasswordHash")
            .HasColumnType("nvarchar(255)")
            .HasMaxLength(255)
            .IsRequired();

        b.Property(x => x.PasswordSalt)
            .HasColumnName("PasswordSalt")
            .HasColumnType("nvarchar(255)")
            .HasMaxLength(255)
            .IsRequired();

        b.Property(x => x.FullName)
            .HasColumnName("FullName")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100)
            .IsRequired();

        // Controle de acesso
        b.Property(x => x.EmailConfirmed)
            .HasColumnName("EmailConfirmed")
            .HasColumnType("bit")
            .IsRequired()
            .HasDefaultValue(false);

        b.Property(x => x.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .IsRequired()
            .HasDefaultValue(true);

        // Tokens de segurança
        b.Property(x => x.EmailConfirmationToken)
            .HasColumnName("EmailConfirmationToken")
            .HasColumnType("nvarchar(255)")
            .HasMaxLength(255)
            .IsRequired(false);

        b.Property(x => x.PasswordResetToken)
            .HasColumnName("PasswordResetToken")
            .HasColumnType("nvarchar(255)")
            .HasMaxLength(255)
            .IsRequired(false);

        b.Property(x => x.PasswordResetTokenExpiry)
            .HasColumnName("PasswordResetTokenExpiry")
            .HasColumnType("datetime2")
            .IsRequired(false);

        // Multi-tenant
        b.Property(x => x.TenantId)
            .HasColumnName("TenantId")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        // Auditoria e controle
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

        b.Property(x => x.LastLoginAt)
            .HasColumnName("LastLoginAt")
            .HasColumnType("datetime2")
            .IsRequired(false);

        b.Property(x => x.LoginAttempts)
            .HasColumnName("LoginAttempts")
            .HasColumnType("int")
            .IsRequired()
            .HasDefaultValue(0);

        b.Property(x => x.LockedUntil)
            .HasColumnName("LockedUntil")
            .HasColumnType("datetime2")
            .IsRequired(false);

        // Metadados
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

        b.Property(x => x.UserAgent)
            .HasColumnName("UserAgent")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500)
            .IsRequired(false);

        b.Property(x => x.IpAddress)
            .HasColumnName("IpAddress")
            .HasColumnType("nvarchar(45)")
            .HasMaxLength(45)
            .IsRequired(false);

        // Ignorar propriedades de conveniência
        b.Ignore(x => x.Username);
        b.Ignore(x => x.DisplayName);
        b.Ignore(x => x.IsLocked);
        b.Ignore(x => x.CanLogin);

        // Índices únicos
        b.HasIndex(x => x.EmailNormalized)
            .IsUnique()
            .HasDatabaseName("UK_SaasUsers_EmailNormalized");

        b.HasIndex(x => x.TenantId)
            .HasDatabaseName("IX_SaasUsers_TenantId");

        b.HasIndex(x => x.EmailConfirmationToken)
            .HasDatabaseName("IX_SaasUsers_EmailConfirmationToken")
            .HasFilter("[EmailConfirmationToken] IS NOT NULL");

        b.HasIndex(x => x.PasswordResetToken)
            .HasDatabaseName("IX_SaasUsers_PasswordResetToken")
            .HasFilter("[PasswordResetToken] IS NOT NULL");

        b.HasIndex(x => new { x.IsActive, x.EmailConfirmed })
            .HasDatabaseName("IX_SaasUsers_IsActive_EmailConfirmed");

        // Relacionamento com Tenant
        b.HasOne(x => x.Tenant)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.TenantId)
            .HasConstraintName("FK_SaasUsers_SaasTenants_TenantId")
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento com Invitations
        b.HasMany(x => x.SentInvitations)
            .WithOne(x => x.InvitedBy)
            .HasForeignKey(x => x.InvitedById)
            .HasConstraintName("FK_SaasInvitations_SaasUsers_InvitedById")
            .OnDelete(DeleteBehavior.Restrict);
    }
}