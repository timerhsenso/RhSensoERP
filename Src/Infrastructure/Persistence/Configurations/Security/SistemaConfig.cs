using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.Security;

/// <summary>
/// Mapeamento EF Core para a tabela dbo.tsistema.
/// </summary>
public sealed class SistemaConfig : IEntityTypeConfiguration<Sistema>
{
    public void Configure(EntityTypeBuilder<Sistema> b)
    {
        b.ToTable("tsistema", "dbo");

        b.HasKey(x => x.CdSistema);

        b.Property(x => x.CdSistema)
            .HasColumnName("cdsistema")
            .HasColumnType("char(10)")
            .IsRequired();

        b.Property(x => x.DcSistema)
            .HasColumnName("dcsistema")
            .HasColumnType("varchar(60)")
            .IsRequired();

        b.Property(x => x.Ativo)
            .HasColumnName("ativo")
            .HasColumnType("bit")
            .HasDefaultValue(true)
            .IsRequired();

        // Ignorar BaseEntity.Id que não existe na tabela legacy
        b.Ignore(x => x.Id);

        // Relacionamento com Função
        b.HasMany(x => x.Funcoes)
         .WithOne(x => x.Sistema)
         .HasForeignKey(x => x.CdSistema)
         .OnDelete(DeleteBehavior.Restrict);
    }
}