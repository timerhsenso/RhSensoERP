using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.Security
{
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

            // Se BaseEntity tiver campos que NÃO existem em tsistema:
            b.Ignore(x => x.Id);          // mantém o legado limpo
            // b.Ignore(x => x.TenantId); b.Ignore(x => x.CreatedAt); etc., se existirem

            // Relacionamento com Função (se aplicável)
            b.HasMany(x => x.Funcoes)
             .WithOne(x => x.Sistema!)
             .HasForeignKey(x => x.CdSistema)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
