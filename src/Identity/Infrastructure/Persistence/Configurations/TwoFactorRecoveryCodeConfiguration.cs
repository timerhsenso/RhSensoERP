// RhSensoERP.Identity/Infrastructure/Persistence/TwoFactorRecoveryCodeConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Core.Entities;

namespace RhSensoERP.Identity.Infrastructure.Persistence;

public class TwoFactorRecoveryCodeConfiguration : IEntityTypeConfiguration<TwoFactorRecoveryCode>
{
    public void Configure(EntityTypeBuilder<TwoFactorRecoveryCode> builder)
    {
        builder.ToTable("SEG_TwoFactorRecoveryCode", schema: "dbo");

        // Chave Primária
        builder.HasKey(x => x.Id).HasName("PK_SEG_TwoFactorRecoveryCode");
        builder.Property(x => x.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .ValueGeneratedOnAdd();

        // Índice
        builder.HasIndex(x => x.IdUserSecurity)
            .HasDatabaseName("IX_SEG_TwoFactorRecoveryCode_UserSecurity")
            .HasFilter("[IsUsed] = 0");

        // Propriedades
        builder.Property(x => x.IdUserSecurity).IsRequired();
        builder.Property(x => x.CodeHash).HasMaxLength(500).IsRequired();
        builder.Property(x => x.GeneratedAt).HasColumnType("datetime2(7)").HasDefaultValueSql("GETUTCDATE()").IsRequired();
        builder.Property(x => x.IsUsed).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.UsedAt).HasColumnType("datetime2(7)").IsRequired(false);
        builder.Property(x => x.UsedFromIp).HasMaxLength(45).IsRequired(false);

        // ✅ FIX: Navegação opcional para compatibilidade com query filter
        builder.HasOne(x => x.UserSecurity)
            .WithMany(x => x.RecoveryCodes)
            .HasForeignKey(x => x.IdUserSecurity)
            .HasConstraintName("FK_SEG_TwoFactorRecoveryCode_UserSecurity")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false); // ✅ Navegação opcional
    }
}