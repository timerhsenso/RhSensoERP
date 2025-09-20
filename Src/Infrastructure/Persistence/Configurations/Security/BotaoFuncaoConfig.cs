using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.Security;

/// <summary>
/// Configuração EF Core para entidade BotaoFuncao → tabela btfuncao
/// Baseada no script SQL: bd_rhu_copenor.dbo.btfuncao
/// </summary>
public class BotaoFuncaoConfig : IEntityTypeConfiguration<BotaoFuncao>
{
    public void Configure(EntityTypeBuilder<BotaoFuncao> b)
    {
        // Mapear para tabela btfuncao
        b.ToTable("btfuncao", schema: "dbo");

        // Chave primária COMPOSTA: (cdsistema, cdfuncao, nmbotao)
        b.HasKey(x => new { x.CdSistema, x.CdFuncao, x.NmBotao });

        // Mapeamento exato das colunas do banco
        b.Property(x => x.CdFuncao)
            .HasColumnName("cdfuncao")
            .HasColumnType("varchar(30)")
            .IsRequired();

        b.Property(x => x.CdSistema)
            .HasColumnName("cdsistema")
            .HasColumnType("char(10)")
            .IsRequired();

        b.Property(x => x.NmBotao)
            .HasColumnName("nmbotao")
            .HasColumnType("varchar(30)")
            .IsRequired();

        b.Property(x => x.DcBotao)
            .HasColumnName("dcbotao")
            .HasColumnType("varchar(60)")
            .IsRequired();

        b.Property(x => x.CdAcao)
            .HasColumnName("cdacao")
            .HasColumnType("char(1)")
            .IsRequired();

        // IGNORAR propriedades que não existem na tabela legacy
        b.Ignore(x => x.Id);              // BaseEntity.Id não existe em btfuncao

        // Relacionamento com Funcao (FK composta)
        b.HasOne(x => x.Funcao)
            .WithMany(x => x.Botoes)
            .HasForeignKey(x => new { x.CdSistema, x.CdFuncao })
            .HasPrincipalKey(x => new { x.CdSistema, x.CdFuncao })
            .HasConstraintName("FK_btfuncao_fucn1_cdsistema_cdfuncao")
            .OnDelete(DeleteBehavior.Restrict);

        // Constraint já existe no banco: PK_btfuncao
        // FK já existe no banco: FK_btfuncao_fucn1_cdsistema_cdfuncao
    }
}