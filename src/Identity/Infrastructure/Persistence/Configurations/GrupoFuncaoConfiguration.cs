using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Core.Entities;

namespace RhSensoERP.Identity.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeia <see cref="GrupoFuncao"/> para a tabela hbrh1.
/// Regras principais:
/// - PK composta: (CdSistema, CdGrUser, CdFuncao) — necessária para EF
/// - Campos: CdAcoes (char(20)), CdRestric (char(1)), CdSistema (char(10)?), IdGrupoDeUsuario (GUID?)
/// - FKs:
///   - Para Funcao: (CdSistema, CdFuncao)
///   - Para GrupoDeUsuario: (CdSistema, CdGrUser) como FK composta
/// </summary>
public sealed class GrupoFuncaoConfiguration : IEntityTypeConfiguration<GrupoFuncao>
{
    public void Configure(EntityTypeBuilder<GrupoFuncao> builder)
    {
        builder.ToTable("hbrh1");

        builder.HasKey(e => new { e.CdSistema, e.CdGrUser, e.CdFuncao });

        builder.Property(e => e.CdGrUser)
            .HasColumnName("cdgruser")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.CdFuncao)
            .HasColumnName("cdfuncao")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.CdAcoes)
            .HasColumnName("cdacoes")
            .HasColumnType("char(20)")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.CdRestric)
            .HasColumnName("cdrestric")
            .HasColumnType("char(1)");

        builder.Property(e => e.CdSistema)
            .HasColumnName("cdsistema")
            .HasMaxLength(10)
            .IsFixedLength();

        builder.Property(e => e.IdGrupoDeUsuario)
            .HasColumnName("idgrupodeusuario");

        // FK para Funcao (composta)
        builder.HasOne(e => e.Funcao)
            .WithMany(f => f.GrupoFuncoes)
            .HasForeignKey(e => new { e.CdSistema, e.CdFuncao })
            .OnDelete(DeleteBehavior.Restrict);

        // FK composta para GrupoDeUsuario (aderente ao legado)
        builder.HasOne(e => e.GrupoDeUsuario)
            .WithMany(g => g.GrupoFuncoes)
            .HasPrincipalKey(g => new { g.CdSistema, g.CdGrUser })
            .HasForeignKey(e => new { e.CdSistema, e.CdGrUser })
            .OnDelete(DeleteBehavior.Cascade);

        // Observação: Se desejar também usar o GUID como FK alternativa:
        // builder.HasOne(e => e.GrupoDeUsuario)
        //    .WithMany(g => g.GrupoFuncoes)
        //    .HasPrincipalKey(g => g.Id)
        //    .HasForeignKey(e => e.IdGrupoDeUsuario)
        //    .OnDelete(DeleteBehavior.NoAction);
    }
}
