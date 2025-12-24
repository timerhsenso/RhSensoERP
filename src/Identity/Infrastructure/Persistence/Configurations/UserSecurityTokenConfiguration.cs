// RhSensoERP.Identity/Infrastructure/Persistence/UserSecurityTokenConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Core.Entities;

namespace RhSensoERP.Identity.Infrastructure.Persistence;

public class UserSecurityTokenConfiguration : IEntityTypeConfiguration<UserSecurityToken>
{
    public void Configure(EntityTypeBuilder<UserSecurityToken> builder)
    {
        builder.ToTable("SEG_UserSecurityToken", schema: "dbo");

        // Chave Primária
        builder.HasKey(x => x.Id).HasName("PK_SEG_UserSecurityToken");
        builder.Property(x => x.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .ValueGeneratedOnAdd();

        // Índice
        builder.HasIndex(x => new { x.TokenHash, x.TokenType })
            .HasDatabaseName("IX_SEG_UserSecurityToken_Hash_Type")
            .HasFilter("[IsUsed] = 0 AND [ExpiresAt] > GETUTCDATE()");

        // Propriedades
        builder.Property(x => x.IdUserSecurity).IsRequired();
        builder.Property(x => x.TokenType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.TokenHash).HasMaxLength(500).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2(7)").HasDefaultValueSql("GETUTCDATE()").IsRequired();
        builder.Property(x => x.ExpiresAt).HasColumnType("datetime2(7)").IsRequired();
        builder.Property(x => x.RequestedFromIp).HasMaxLength(45).IsRequired(false);
        builder.Property(x => x.IsUsed).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.UsedAt).HasColumnType("datetime2(7)").IsRequired(false);
        builder.Property(x => x.UsedFromIp).HasMaxLength(45).IsRequired(false);

        // ✅ FIX: Navegação opcional para compatibilidade com query filter
        builder.HasOne(x => x.UserSecurity)
            .WithMany(x => x.SecurityTokens)
            .HasForeignKey(x => x.IdUserSecurity)
            .HasConstraintName("FK_SEG_UserSecurityToken_UserSecurity")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false); // ✅ Navegação opcional
    }
}