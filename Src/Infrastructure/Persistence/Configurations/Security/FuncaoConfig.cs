using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class FuncaoConfig : IEntityTypeConfiguration<Funcao>
{
    public void Configure(EntityTypeBuilder<Funcao> b)
    {
        b.ToTable("fucn1", schema: "dbo");

        // Chave primária composta
        b.HasKey(x => new { x.CdSistema, x.CdFuncao });

        b.Property(x => x.CdFuncao).HasColumnName("cdfuncao").HasMaxLength(30).IsRequired();
        b.Property(x => x.CdSistema).HasColumnName("cdsistema").HasMaxLength(10).IsRequired();
        b.Property(x => x.DcFuncao).HasColumnName("dcfuncao").HasMaxLength(80);
        b.Property(x => x.DcModulo).HasColumnName("dcmodulo").HasMaxLength(100);
        b.Property(x => x.DescricaoModulo).HasColumnName("descricaomodulo").HasMaxLength(100);

        // Relacionamento com Sistema
        b.HasOne(x => x.Sistema)
         .WithMany(x => x.Funcoes)
         .HasForeignKey(x => x.CdSistema)
         .HasConstraintName("fk_fucn1_cdsistema");

        b.Ignore(x => x.Id); // Não usar Guid, usar chave composta
    }
}