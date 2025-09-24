// Auto-generated EF Core configuration for RHU module.
// Maps to legacy table names via ToTable("...") and defines keys, FKs, sizes and nullability.
// Review column types (HasColumnType) if you need exact database types.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.RHU.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.RHU
{
    /// <summary>
    /// Tabela tcon2 — contém as verbas/contas usadas nos cálculos de folha.
    /// </summary>
    public class VerbaConfiguration : IEntityTypeConfiguration<Verba>
    {
        public void Configure(EntityTypeBuilder<Verba> b)
        {
            b.ToTable("tcon2");
            b.HasKey(x => x.CdConta);

            b.Property(x => x.CdConta).HasMaxLength(4).IsRequired();
            b.Property(x => x.DcConta).HasMaxLength(40).IsRequired();
            b.Property(x => x.SgConta).HasMaxLength(10);
            b.Property(x => x.UfConta).HasMaxLength(2);
            b.Property(x => x.CdFormula).HasMaxLength(20);
            b.Property(x => x.CdTabela).HasMaxLength(10);
            b.Property(x => x.CdContab).HasMaxLength(20);
            b.Property(x => x.ChHis).HasMaxLength(10);
            b.Property(x => x.ChFol).HasMaxLength(10);
            b.Property(x => x.ChDem).HasMaxLength(10);
            b.Property(x => x.ChInfor).HasMaxLength(10);
            b.Property(x => x.ChLanc).HasMaxLength(10);
            b.Property(x => x.CdTrct).HasMaxLength(10);
            b.Property(x => x.ChTrct).HasMaxLength(10);
            b.Property(x => x.CdContaBc).HasMaxLength(4);
        }
    }
}
