using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Core.Entities;

namespace RhSensoERP.Identity.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeia <see cref="BotaoFuncao"/> para a tabela dbo.btfuncao.
/// PK composta: (cdsistema, cdfuncao, nmbotao)
/// FK: (cdsistema, cdfuncao) -> dbo.fucn1(cdsistema, cdfuncao)
/// </summary>
public sealed class BotaoFuncaoConfiguration : IEntityTypeConfiguration<BotaoFuncao>
{
    public void Configure(EntityTypeBuilder<BotaoFuncao> builder)
    {
        builder.ToTable("btfuncao", "dbo");

        builder.HasKey(e => new { e.CdSistema, e.CdFuncao, e.NmBotao })
               .HasName("PK_btfuncao");

        builder.Property(e => e.CdSistema)
            .HasColumnName("cdsistema")
            .HasColumnType("char(10)")
            .IsFixedLength()
            .IsRequired();

        builder.Property(e => e.CdFuncao)
            .HasColumnName("cdfuncao")
            .HasColumnType("varchar(30)")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.NmBotao)
            .HasColumnName("nmbotao")
            .HasColumnType("varchar(30)")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.DcBotao)
            .HasColumnName("dcbotao")
            .HasColumnType("varchar(60)")
            .HasMaxLength(60)
            .IsRequired();

        // Importante: no banco é char(1) NOT NULL.
        // Como a propriedade é "char", EF armazena como string(1).
        builder.Property(e => e.CdAcao)
            .HasColumnName("cdacao")
            .HasColumnType("char(1)")
            .IsFixedLength()
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => string.IsNullOrWhiteSpace(v) ? '\0' : v[0]);

        // FK composta conforme o banco (ajuste de delete abaixo)
        builder.HasOne(e => e.Funcao)
            .WithMany(f => f.Botoes)
            .HasForeignKey(e => new { e.CdSistema, e.CdFuncao })
            .HasConstraintName("FK_btfuncao_fucn1_cdsistema_cdfuncao")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
