// src/Identity/Infrastructure/Persistence/Configurations/SecurityAuditLogConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Core.Entities;

namespace RhSensoERP.Identity.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração EF Core para SecurityAuditLog.
/// ✅ FASE 5: Mapeamento completo da tabela de auditoria
/// </summary>
public class SecurityAuditLogConfiguration : IEntityTypeConfiguration<SecurityAuditLog>
{
    public void Configure(EntityTypeBuilder<SecurityAuditLog> builder)
    {
        // ✅ MUDANÇA: Prefixo SEG_ para tabelas de segurança
        builder.ToTable("SEG_SecurityAuditLogs");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Event Info
        builder.Property(e => e.EventType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.EventCategory)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Severity)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(1000);

        // Timestamp
        builder.Property(e => e.OccurredAt)
            .IsRequired();

        // User Info
        builder.Property(e => e.IdUserSecurity)
            .IsRequired(false);

        builder.Property(e => e.Username)
            .HasMaxLength(100);

        // Request Context
        builder.Property(e => e.IpAddress)
            .IsRequired()
            .HasMaxLength(45); // IPv6 max length

        builder.Property(e => e.UserAgent)
            .HasMaxLength(500);

        builder.Property(e => e.RequestPath)
            .HasMaxLength(500);

        builder.Property(e => e.RequestMethod)
            .HasMaxLength(10);

        // Details
        builder.Property(e => e.AdditionalData)
            .HasMaxLength(4000);

        builder.Property(e => e.IsSuccess)
            .IsRequired();

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(1000);

        // Relacionamento com UserSecurity
        builder.HasOne(e => e.UserSecurity)
            .WithMany()
            .HasForeignKey(e => e.IdUserSecurity)
            .OnDelete(DeleteBehavior.SetNull);

        // Índices para performance
        builder.HasIndex(e => e.OccurredAt)
            .HasDatabaseName("IX_SecurityAuditLogs_OccurredAt");

        builder.HasIndex(e => e.EventType)
            .HasDatabaseName("IX_SecurityAuditLogs_EventType");

        builder.HasIndex(e => e.Severity)
            .HasDatabaseName("IX_SecurityAuditLogs_Severity");

        builder.HasIndex(e => e.IdUserSecurity)
            .HasDatabaseName("IX_SecurityAuditLogs_IdUserSecurity");

        builder.HasIndex(e => e.IpAddress)
            .HasDatabaseName("IX_SecurityAuditLogs_IpAddress");

        // Índice composto para queries comuns
        builder.HasIndex(e => new { e.OccurredAt, e.EventType, e.Severity })
            .HasDatabaseName("IX_SecurityAuditLogs_OccurredAt_EventType_Severity");
    }
}
