// src/Identity/Infrastructure/Persistence/Configurations/UserGroupConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Core.Entities;

namespace RhSensoERP.Identity.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração EF Core para UserGroup (tabela usrh1).
/// </summary>
public sealed class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> builder)
    {
        builder.ToTable("usrh1");

        // ============================================================
        // CHAVE PRIMÁRIA
        // ============================================================

        // PK composta do legado
        builder.HasKey(e => new { e.CdSistema, e.CdGrUser, e.CdUsuario })
            .HasName("PK_usrh1");

        // ============================================================
        // PROPRIEDADES
        // ============================================================

        builder.Property(e => e.CdUsuario)
            .HasColumnName("cdusuario")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.CdGrUser)
            .HasColumnName("cdgruser")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.CdSistema)
            .HasColumnName("cdsistema")
            .HasMaxLength(10)
            .IsFixedLength()
            .IsRequired();

        builder.Property(e => e.IdGrupoDeUsuario)
            .HasColumnName("idgrupodeusuario");
                

        // ============================================================
        // RELACIONAMENTOS
        // ============================================================

        builder.HasOne(e => e.Usuario)
            .WithMany(u => u.UserGroups)
            .HasForeignKey(e => e.CdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.GrupoDeUsuario)
            .WithMany(g => g.UserGroups)
            .HasPrincipalKey(g => new { g.CdSistema, g.CdGrUser })
            .HasForeignKey(e => new { e.CdSistema, e.CdGrUser })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Sistema)
            .WithMany()
            .HasForeignKey(e => e.CdSistema)
            .OnDelete(DeleteBehavior.Restrict);
    }
}