// src/Identity/Infrastructure/Persistence/Configurations/GrupoDeUsuarioConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Core.Entities;

namespace RhSensoERP.Identity.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração EF Core para GrupoDeUsuario (tabela gurh1).
/// </summary>
public sealed class GrupoDeUsuarioConfiguration : IEntityTypeConfiguration<GrupoDeUsuario>
{
    public void Configure(EntityTypeBuilder<GrupoDeUsuario> builder)
    {
        builder.ToTable("gurh1");

        // ============================================================
        // CHAVES
        // ============================================================

        // PK composta conforme padrão legado
        builder.HasKey(e => new { e.CdSistema, e.CdGrUser })
            .HasName("PK_gurh1");

        // Alternate Key no Id para integrações modernas
        /*builder.HasAlternateKey(e => e.Id)
            .HasName("AK_gurh1_Id"); */

        // ============================================================
        // PROPRIEDADES
        // ============================================================

        builder.Property(e => e.CdGrUser)
            .HasColumnName("cdgruser")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.DcGrUser)
            .HasColumnName("dcgruser")
            .HasMaxLength(60);

        builder.Property(e => e.CdSistema)
            .HasColumnName("cdsistema")
            .HasMaxLength(10)
            .IsRequired()
            .IsFixedLength();

        /*builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd(); */
                
        // ============================================================
        // RELACIONAMENTOS
        // ============================================================

        builder.HasOne(e => e.Sistema)
            .WithMany()
            .HasForeignKey(e => e.CdSistema)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.GrupoFuncoes)
            .WithOne(gf => gf.GrupoDeUsuario!)
            .HasPrincipalKey(e => new { e.CdSistema, e.CdGrUser })
            .HasForeignKey(gf => new { gf.CdSistema, gf.CdGrUser })
            .OnDelete(DeleteBehavior.Cascade);
    }
}