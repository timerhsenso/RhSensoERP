// RhSensoERP.Identity/Infrastructure/Persistence/UserSecurityConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Core.Entities;

namespace RhSensoERP.Identity.Infrastructure.Persistence.Configurations;

public class UserSecurityConfiguration : IEntityTypeConfiguration<UserSecurity>
{
    public void Configure(EntityTypeBuilder<UserSecurity> builder)
    {
        // ✅ CORREÇÃO CRÍTICA: Informar que a tabela possui triggers
        // Isso desabilita a cláusula OUTPUT e usa SELECT após INSERT/UPDATE
        builder.ToTable("SEG_UserSecurity", "dbo", tb => tb.HasTrigger("TR_UserSecurity_Audit"));

        // Chave Primária
        builder.HasKey(x => x.Id).HasName("PK_SEG_UserSecurity");
        builder.Property(x => x.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .ValueGeneratedOnAdd();

        // Unique Constraint (1:1 com tuse1)
        builder.HasIndex(x => x.IdUsuario)
            .IsUnique()
            .HasDatabaseName("UK_SEG_UserSecurity_IdUsuario");

        // Índices
        builder.HasIndex(x => x.IdSaaS)
            .HasDatabaseName("IX_SEG_UserSecurity_IdSaaS")
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(x => x.SecurityStamp)
            .HasDatabaseName("IX_SEG_UserSecurity_SecurityStamp");

        builder.HasIndex(x => x.LockoutEnd)
            .HasDatabaseName("IX_SEG_UserSecurity_LockoutEnd")
            .HasFilter("[LockoutEnd] IS NOT NULL AND [IsDeleted] = 0");

        // Propriedades - Identidade
        builder.Property(x => x.IdUsuario).IsRequired();
        builder.Property(x => x.IdSaaS).IsRequired(false);

        // Propriedades - Senha
        builder.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
        builder.Property(x => x.PasswordSalt).HasMaxLength(200).IsRequired();
        builder.Property(x => x.PasswordAlgorithm).HasMaxLength(50).HasDefaultValue("PBKDF2-SHA512").IsRequired();
        builder.Property(x => x.PasswordVersion).HasDefaultValue(1).IsRequired();
        builder.Property(x => x.PasswordChangedAt).HasColumnType("datetime2(7)").HasDefaultValueSql("GETUTCDATE()").IsRequired();
        builder.Property(x => x.MustChangePassword).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.ForcePasswordChangeReason).HasMaxLength(500).IsRequired(false);

        // Propriedades - Lockout
        builder.Property(x => x.AccessFailedCount).HasDefaultValue(0).IsRequired();
        builder.Property(x => x.LockoutEnd).HasColumnType("datetime2(7)").IsRequired(false);
        builder.Property(x => x.LockoutEnabled).HasDefaultValue(true).IsRequired();
        builder.Property(x => x.AccountLockedReason).HasMaxLength(500).IsRequired(false);

        // Propriedades - 2FA
        builder.Property(x => x.TwoFactorEnabled).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.TwoFactorSecret).HasMaxLength(500).IsRequired(false);
        builder.Property(x => x.TwoFactorType).HasMaxLength(20).IsRequired(false);
        builder.Property(x => x.TwoFactorActivatedAt).HasColumnType("datetime2(7)").IsRequired(false);

        // Propriedades - Confirmações
        builder.Property(x => x.EmailConfirmed).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.EmailConfirmedAt).HasColumnType("datetime2(7)").IsRequired(false);
        builder.Property(x => x.PhoneNumber).HasMaxLength(20).IsRequired(false);
        builder.Property(x => x.PhoneNumberConfirmed).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.PhoneNumberConfirmedAt).HasColumnType("datetime2(7)").IsRequired(false);

        // Propriedades - Security Stamp
        builder.Property(x => x.SecurityStamp).HasMaxLength(100).HasDefaultValueSql("NEWID()").IsRequired();
        builder.Property(x => x.ConcurrencyStamp).HasMaxLength(100).HasDefaultValueSql("NEWID()").IsRequired().IsConcurrencyToken();

        // Propriedades - Cache
        builder.Property(x => x.LastLoginAt).HasColumnType("datetime2(7)").IsRequired(false);
        builder.Property(x => x.LastLoginIP).HasMaxLength(45).IsRequired(false);

        // Propriedades - Auditoria
        builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired();
        builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2(7)").HasDefaultValueSql("GETUTCDATE()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime2(7)").HasDefaultValueSql("GETUTCDATE()").IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(30).IsRequired(false);
        builder.Property(x => x.UpdatedBy).HasMaxLength(30).IsRequired(false);
        builder.Property(x => x.DeletedAt).HasColumnType("datetime2(7)").IsRequired(false);
        builder.Property(x => x.DeletedBy).HasMaxLength(30).IsRequired(false);

        // Navegação
        builder.HasMany(x => x.PasswordHistories)
            .WithOne(x => x.UserSecurity)
            .HasForeignKey(x => x.IdUserSecurity)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.LoginAuditLogs)
            .WithOne(x => x.UserSecurity)
            .HasForeignKey(x => x.IdUserSecurity)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.RefreshTokens)
            .WithOne(x => x.UserSecurity)
            .HasForeignKey(x => x.IdUserSecurity)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.RecoveryCodes)
            .WithOne(x => x.UserSecurity)
            .HasForeignKey(x => x.IdUserSecurity)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.SecurityTokens)
            .WithOne(x => x.UserSecurity)
            .HasForeignKey(x => x.IdUserSecurity)
            .OnDelete(DeleteBehavior.Cascade);

        // Query Filter (Soft Delete)
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}