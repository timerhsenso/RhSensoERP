using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Core.Entities;

namespace RhSensoERP.Identity.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeia a entidade <see cref="Funcao"/> para a tabela fucn1.
/// Regras principais:
/// - PK composta: (CdSistema, CdFuncao)
/// - Campos: DcFuncao (varchar(80)?), DcModulo/DescricaoModulo (varchar(100)?)
/// - FK: Sistema(CdSistema)
/// - Relacionamentos: 1:N com BotaoFuncao; 1:N com GrupoFuncao
/// </summary>
public sealed class FuncaoConfiguration : IEntityTypeConfiguration<Funcao>
{
    public void Configure(EntityTypeBuilder<Funcao> builder)
    {
                

        builder.ToTable("fucn1");

        builder.HasKey(e => new { e.CdSistema, e.CdFuncao });

        builder.Property(e => e.CdFuncao)
            .HasColumnName("cdfuncao")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.CdSistema)
            .HasColumnName("cdsistema")
            .HasMaxLength(10)
            .IsRequired()
            .IsFixedLength();

        builder.Property(e => e.DcFuncao)
            .HasColumnName("dcfuncao")
            .HasMaxLength(80);

        builder.Property(e => e.DcModulo)
            .HasColumnName("dcmodulo")
            .HasMaxLength(100);

        builder.Property(e => e.DescricaoModulo)
            .HasColumnName("descricaomodulo")
            .HasMaxLength(100);

        builder.HasOne(e => e.Sistema)
            .WithMany(s => s.Funcoes)
            .HasForeignKey(e => e.CdSistema)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Botoes)
            .WithOne(b => b.Funcao)
            .HasForeignKey(b => new { b.CdSistema, b.CdFuncao })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.GrupoFuncoes)
            .WithOne(gf => gf.Funcao!)
            .HasForeignKey(gf => new { gf.CdSistema, gf.CdFuncao })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
