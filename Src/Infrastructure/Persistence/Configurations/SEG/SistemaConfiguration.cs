using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.SEG
{
    public sealed class SistemaConfiguration : IEntityTypeConfiguration<Sistema>
    {
        public void Configure(EntityTypeBuilder<Sistema> b)
        {
            b.ToTable("tsistema");
            b.HasKey(x => x.CdSistema);

            b.Property(x => x.CdSistema).HasColumnName("cdsistema").HasMaxLength(10).IsRequired();
            b.Property(x => x.DcSistema).HasColumnName("dcsistema").HasMaxLength(60).IsRequired();
            b.Property(x => x.Ativo).HasColumnName("ativo").HasDefaultValue(true).IsRequired();

            b.HasMany(x => x.Funcoes)
                .WithOne(x => x.Sistema!)
                .HasForeignKey(x => x.CdSistema)
                .OnDelete(DeleteBehavior.Restrict);

            /*b.Ignore(x => x.TenantId);
            b.Ignore(x => x.IsDeleted);
            b.Ignore(x => x.CreatedAt);
            b.Ignore(x => x.CreatedBy);
            b.Ignore(x => x.UpdatedAt);
            b.Ignore(x => x.UpdatedBy);
            b.Ignore(x => x.Id);
            b.Ignore(x => x.RowVersion);*/
        }
    }
}