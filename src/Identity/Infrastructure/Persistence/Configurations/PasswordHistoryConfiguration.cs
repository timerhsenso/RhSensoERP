// RhSensoERP.Identity/Infrastructure/Persistence/PasswordHistoryConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Core.Entities;

namespace RhSensoERP.Identity.Infrastructure.Persistence;

public class PasswordHistoryConfiguration : IEntityTypeConfiguration<PasswordHistory>
{
    public void Configure(EntityTypeBuilder<PasswordHistory> builder)
    {
        builder.ToTable("SEG_PasswordHistory", schema: "dbo");

        // Chave Primária
        builder.HasKey(x => x.Id).HasName("PK_SEG_PasswordHistory");
        builder.Property(x => x.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .ValueGeneratedOnAdd();

        // Índice
        builder.HasIndex(x => new { x.IdUserSecurity, x.ChangedAt })
            .HasDatabaseName("IX_SEG_PasswordHistory_UserSecurity_ChangedAt")
            .IsDescending(false, true);

        // Propriedades
        builder.Property(x => x.IdUserSecurity).IsRequired();
        builder.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
        builder.Property(x => x.PasswordAlgorithm).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ChangedAt).HasColumnType("datetime2(7)").HasDefaultValueSql("GETUTCDATE()").IsRequired();
        builder.Property(x => x.ChangedByIP).HasMaxLength(45).IsRequired(false);
        builder.Property(x => x.ChangeReason).HasMaxLength(50).IsRequired(false);

        // ✅ FIX: Navegação opcional para compatibilidade com query filter
        builder.HasOne(x => x.UserSecurity)
            .WithMany(x => x.PasswordHistories)
            .HasForeignKey(x => x.IdUserSecurity)
            .HasConstraintName("FK_SEG_PasswordHistory_UserSecurity")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false); // ✅ Navegação opcional
    }
}