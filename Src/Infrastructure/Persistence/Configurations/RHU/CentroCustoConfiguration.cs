// Auto-generated EF Core configuration for RHU module.
// Maps to legacy table names via ToTable("...") and defines keys, FKs, sizes and nullability.
// Review column types (HasColumnType) if you need exact database types.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.RHU.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.RHU
{
    /// <summary>
    /// Tabela tcus1 — contém os centros de custo.
    /// </summary>
    public class CentroCustoConfiguration : IEntityTypeConfiguration<CentroCusto>
    {
        public void Configure(EntityTypeBuilder<CentroCusto> b)
        {
            b.ToTable("tcus1");
            b.HasKey(x => x.CdCcusto);

            b.Property(x => x.CdCcusto).HasMaxLength(20).IsRequired();
            b.Property(x => x.DcCcusto).HasMaxLength(100).IsRequired();
            b.Property(x => x.SgCcusto).HasMaxLength(20);
            b.Property(x => x.NoCcusto).HasMaxLength(50).IsRequired();
            b.Property(x => x.DcAreaCracha).HasMaxLength(100);
        }
    }
}
