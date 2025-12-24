// src/Identity/Infrastructure/Persistence/Configurations/LoginAuditLogConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Core.Entities;

namespace RhSensoERP.Identity.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração EF Core para LoginAuditLog (tabela SEG_LoginAuditLog).
/// </summary>
public sealed class LoginAuditLogConfiguration : IEntityTypeConfiguration<LoginAuditLog>
{
    public void Configure(EntityTypeBuilder<LoginAuditLog> builder)
    {
        builder.ToTable("SEG_LoginAuditLog", schema: "dbo");

        // ============================================================
        // CHAVE PRIMÁRIA
        // ============================================================
        builder.HasKey(x => x.Id)
            .HasName("PK_SEG_LoginAuditLog");

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();

        // ============================================================
        // ÍNDICES
        // ============================================================
        builder.HasIndex(x => new { x.IdUserSecurity, x.LoginAttemptAt })
            .HasDatabaseName("IX_SEG_LoginAuditLog_UserSecurity_Date")
            .IsDescending(false, true);

        builder.HasIndex(x => new { x.IdSaaS, x.LoginAttemptAt })
            .HasDatabaseName("IX_SEG_LoginAuditLog_IdSaaS_Date")
            .HasFilter("[IdSaaS] IS NOT NULL")
            .IsDescending(false, true);

        builder.HasIndex(x => new { x.IpAddress, x.LoginAttemptAt })
            .HasDatabaseName("IX_SEG_LoginAuditLog_IP_Failed")
            .HasFilter("[IsSuccess] = 0")
            .IsDescending(false, true);

        // ============================================================
        // PROPRIEDADES
        // ============================================================
        builder.Property(x => x.IdUserSecurity)
            .IsRequired();

        builder.Property(x => x.IdSaaS)
            .IsRequired(false);

        builder.Property(x => x.LoginAttemptAt)
            .HasColumnType("datetime2(7)")
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(x => x.IsSuccess)
            .IsRequired();

        builder.Property(x => x.FailureReason)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(45)
            .IsRequired();

        builder.Property(x => x.UserAgent)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.DeviceType)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.Location)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.TwoFactorUsed)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.SessionId)
            .HasMaxLength(100)
            .IsRequired(false);

        // ============================================================
        // RELACIONAMENTOS
        // ============================================================

        // ✅ FIX: Navegação OPCIONAL para evitar problemas com Global Query Filter
        builder.HasOne(x => x.UserSecurity)
            .WithMany(x => x.LoginAuditLogs)
            .HasForeignKey(x => x.IdUserSecurity)
            .HasConstraintName("FK_SEG_LoginAuditLog_UserSecurity")
            .OnDelete(DeleteBehavior.Restrict) // ✅ Restrict em vez de Cascade
            .IsRequired(false); // ✅ OPCIONAL para compatibilidade com query filter

        // ✅ FIX: Adicionar Global Query Filter para respeitar soft delete do UserSecurity
        builder.HasQueryFilter(e =>
            e.UserSecurity == null || !e.UserSecurity.IsDeleted);
    }
}