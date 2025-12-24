// ============================================================================
// ARQUIVO ALTERADO - SUBSTITUIR: src/Identity/Infrastructure/Persistence/Configurations/SecurityPolicyConfiguration.cs
// ============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Core.Entities;

namespace RhSensoERP.Identity.Infrastructure.Persistence;

public class SecurityPolicyConfiguration : IEntityTypeConfiguration<SecurityPolicy>
{
    public void Configure(EntityTypeBuilder<SecurityPolicy> builder)
    {
        builder.ToTable("SEG_SecurityPolicy", schema: "dbo");

        // Chave Primária
        builder.HasKey(x => x.Id).HasName("PK_SEG_SecurityPolicy");
        builder.Property(x => x.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .ValueGeneratedOnAdd();

        // Unique Constraint
        builder.HasIndex(x => new { x.IdSaaS, x.PolicyName })
            .IsUnique()
            .HasDatabaseName("UK_SEG_SecurityPolicy_TenantName");

        // Propriedades
        builder.Property(x => x.IdSaaS).IsRequired(false);
        builder.Property(x => x.PolicyName).HasMaxLength(100).IsRequired();

        // ✅ NOVO - FASE 1: Modo de Autenticação
        builder.Property(x => x.AuthMode)
            .HasMaxLength(20)
            .IsRequired(false)
            .HasComment("Modo de autenticação: Legacy, SaaS, ADWin. NULL = usar configuração padrão do sistema.");

        builder.Property(x => x.PasswordMinLength).HasDefaultValue(8).IsRequired();
        builder.Property(x => x.PasswordRequireDigit).HasDefaultValue(true).IsRequired();
        builder.Property(x => x.PasswordRequireUppercase).HasDefaultValue(true).IsRequired();
        builder.Property(x => x.PasswordRequireLowercase).HasDefaultValue(true).IsRequired();
        builder.Property(x => x.PasswordRequireNonAlphanumeric).HasDefaultValue(true).IsRequired();
        builder.Property(x => x.PasswordExpirationDays).HasDefaultValue(90).IsRequired(false);
        builder.Property(x => x.PasswordHistoryCount).HasDefaultValue(5).IsRequired();

        builder.Property(x => x.MaxFailedAccessAttempts).HasDefaultValue(5).IsRequired();
        builder.Property(x => x.LockoutDurationMinutes).HasDefaultValue(30).IsRequired();
        builder.Property(x => x.ResetFailedCountAfterMinutes).HasDefaultValue(15).IsRequired();

        builder.Property(x => x.SessionTimeoutMinutes).HasDefaultValue(30).IsRequired();
        builder.Property(x => x.RefreshTokenExpirationDays).HasDefaultValue(7).IsRequired();
        builder.Property(x => x.RequireTwoFactorForAdmins).HasDefaultValue(false).IsRequired();

        builder.Property(x => x.AllowConcurrentSessions).HasDefaultValue(true).IsRequired();
        builder.Property(x => x.MaxConcurrentSessions).HasDefaultValue(3).IsRequired();

        builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2(7)").HasDefaultValueSql("GETUTCDATE()").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime2(7)").HasDefaultValueSql("GETUTCDATE()").IsRequired();

        // ✅ NOVO - FASE 1: Índice para AuthMode
        builder.HasIndex(x => x.AuthMode)
            .HasDatabaseName("IX_SEG_SecurityPolicy_AuthMode")
            .HasFilter("[AuthMode] IS NOT NULL");

        // Seed Data (comentado - usar migration separada para seed)
        // builder.HasData(new SecurityPolicy("DefaultGlobal", null));
    }
}
