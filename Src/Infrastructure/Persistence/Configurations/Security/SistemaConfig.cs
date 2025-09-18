using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SistemaConfig : IEntityTypeConfiguration<Sistema>
{
    public void Configure(EntityTypeBuilder<Sistema> b)
    {
        b.ToTable("tsistema", schema: "dbo");

        b.HasKey(x => x.CdSistema);

        b.Property(x => x.CdSistema).HasColumnName("cdsistema").HasMaxLength(10).IsRequired();
        b.Property(x => x.DcSistema).HasColumnName("dcsistema").HasMaxLength(60).IsRequired();
        b.Property(x => x.Ativo).HasColumnName("ativo").HasDefaultValue(true);

        // Não mapear a propriedade Id (Guid) herdada, usar apenas CdSistema
        b.Ignore(x => x.Id);
    }
}