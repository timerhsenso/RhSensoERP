// RhSensoERP.Identity/Infrastructure/Persistence/RefreshTokenConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Core.Entities;

namespace RhSensoERP.Identity.Infrastructure.Persistence;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        // ✅ CORREÇÃO CRÍTICA: Informar que a tabela possui triggers
        // Isso desabilita a cláusula OUTPUT e usa SELECT após INSERT/UPDATE
        builder.ToTable("SEG_RefreshToken", "dbo", tb => tb.HasTrigger("TR_RefreshToken_Audit"));

        // Chave Primária
        builder.HasKey(x => x.Id).HasName("PK_SEG_RefreshToken");
        builder.Property(x => x.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .ValueGeneratedOnAdd();

        // Índices
        builder.HasIndex(x => x.TokenHash)
            .HasDatabaseName("IX_SEG_RefreshToken_TokenHash")
            .HasFilter("[IsRevoked] = 0 AND [ExpiresAt] > GETUTCDATE()");

        builder.HasIndex(x => x.IdUserSecurity)
            .HasDatabaseName("IX_SEG_RefreshToken_UserSecurity")
            .IncludeProperties(x => new { x.DeviceName, x.CreatedAt })
            .HasFilter("[IsRevoked] = 0");

        // Propriedades
        builder.Property(x => x.IdUserSecurity).IsRequired();
        builder.Property(x => x.TokenHash).HasMaxLength(500).IsRequired();
        builder.Property(x => x.DeviceId).HasMaxLength(100).IsRequired(false);
        builder.Property(x => x.DeviceName).HasMaxLength(200).IsRequired(false);
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2(7)").HasDefaultValueSql("GETUTCDATE()").IsRequired();
        builder.Property(x => x.ExpiresAt).HasColumnType("datetime2(7)").IsRequired();
        builder.Property(x => x.CreatedByIp).HasMaxLength(45).IsRequired();
        builder.Property(x => x.IsRevoked).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.RevokedAt).HasColumnType("datetime2(7)").IsRequired(false);
        builder.Property(x => x.RevokedByIp).HasMaxLength(45).IsRequired(false);
        builder.Property(x => x.RevokedReason).HasMaxLength(100).IsRequired(false);
        builder.Property(x => x.ReplacedByTokenId).IsRequired(false);

        // ✅ FIX: Relacionamento opcional para evitar warning com query filter
        // UserSecurity tem query filter (!IsDeleted), então a navegação deve ser opcional
        builder.HasOne(x => x.UserSecurity)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.IdUserSecurity)
            .HasConstraintName("FK_SEG_RefreshToken_UserSecurity")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false); // ✅ Navegação opcional

        builder.HasOne(x => x.ReplacedByToken)
            .WithMany()
            .HasForeignKey(x => x.ReplacedByTokenId)
            .HasConstraintName("FK_SEG_RefreshToken_ReplacedBy")
            .OnDelete(DeleteBehavior.NoAction);

        // ✅ ALTERNATIVA: Adicionar query filter correspondente (comentado)
        // builder.HasQueryFilter(rt => !rt.UserSecurity.IsDeleted);
    }
}