using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.Security;

/// <summary>
/// Configuração EF Core para entidade Funcao → tabela fucn1
/// Baseada no script SQL: bd_rhu_copenor.dbo.fucn1
/// </summary>
public class FuncaoConfig : IEntityTypeConfiguration<Funcao>
{
    public void Configure(EntityTypeBuilder<Funcao> b)
    {
        // Mapear para tabela fucn1
        b.ToTable("fucn1", schema: "dbo");

        // Chave primária COMPOSTA: (cdsistema, cdfuncao)
        b.HasKey(x => new { x.CdSistema, x.CdFuncao });

        // Mapeamento exato das colunas do banco
        b.Property(x => x.CdFuncao)
            .HasColumnName("cdfuncao")
            .HasColumnType("varchar(30)")
            .IsRequired();

        b.Property(x => x.CdSistema)
            .HasColumnName("cdsistema")
            .HasColumnType("char(10)")
            .IsRequired();

        b.Property(x => x.DcFuncao)
            .HasColumnName("dcfuncao")
            .HasColumnType("varchar(80)")
            .IsRequired(false);             // NULL permitido

        b.Property(x => x.DcModulo)
            .HasColumnName("dcmodulo")
            .HasColumnType("varchar(100)")
            .IsRequired(false);             // NULL permitido

        b.Property(x => x.DescricaoModulo)
            .HasColumnName("descricaomodulo")
            .HasColumnType("varchar(100)")
            .IsRequired(false);             // NULL permitido

        // IGNORAR propriedades que não existem na tabela legacy
        b.Ignore(x => x.Id);              // BaseEntity.Id não existe em fucn1

        // Relacionamento com Sistema (FK)
        b.HasOne(x => x.Sistema)
            .WithMany(x => x.Funcoes)
            .HasForeignKey(x => x.CdSistema)
            .HasPrincipalKey(x => x.CdSistema)
            .HasConstraintName("fk_fucn1_cdsistema")
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamentos com outras entidades (configuraremos depois)
        b.HasMany(x => x.Botoes)
            .WithOne(x => x.Funcao)
            .HasForeignKey(x => new { x.CdSistema, x.CdFuncao })
            .HasPrincipalKey(x => new { x.CdSistema, x.CdFuncao });

        b.HasMany(x => x.GrupoFuncoes)
            .WithOne() // Sem propriedade de navegação reversa por enquanto
            .HasForeignKey(x => new { x.CdSistema, x.CdFuncao })
            .HasPrincipalKey(x => new { x.CdSistema, x.CdFuncao });

        // Constraint já existe no banco: pk_fucn1_todos
        // FK já existe no banco: fk_fucn1_cdsistema
    }
}