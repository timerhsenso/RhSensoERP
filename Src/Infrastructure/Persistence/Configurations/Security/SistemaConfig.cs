using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.Security;

/// <summary>
/// Configuração EF Core para entidade Sistema → tabela tsistema
/// Baseada no script SQL: bd_rhu_copenor.dbo.tsistema
/// </summary>
public class SistemaConfig : IEntityTypeConfiguration<Sistema>
{
    public void Configure(EntityTypeBuilder<Sistema> b)
    {
        // Mapear para tabela tsistema
        b.ToTable("tsistema", schema: "dbo");

        // Chave primária: cdsistema (char(10) NOT NULL)
        b.HasKey(x => x.CdSistema);

        // Mapeamento exato das colunas do banco
        b.Property(x => x.CdSistema)
            .HasColumnName("cdsistema")
            .HasColumnType("char(10)")      // Exatamente como no banco
            .IsRequired();

        b.Property(x => x.DcSistema)
            .HasColumnName("dcsistema")
            .HasColumnType("varchar(60)")   // Exatamente como no banco (60, não 100)
            .IsRequired();

        b.Property(x => x.Ativo)
            .HasColumnName("ativo")
            .HasColumnType("bit")           // Exatamente como no banco
            .IsRequired()
            .HasDefaultValue(true);         // DEFAULT ((1))

        // IGNORAR propriedades que não existem na tabela legacy
        b.Ignore(x => x.Id);              // BaseEntity.Id não existe em tsistema

        // Relacionamentos (configuraremos quando fizer Funcao)
        b.HasMany(x => x.Funcoes)
            .WithOne(x => x.Sistema)
            .HasForeignKey(x => x.CdSistema)
            .HasPrincipalKey(x => x.CdSistema);

        // Constraint já existe no banco: pk_tsistema_cdsistema
    }
}