// Auto-generated EF Core configuration for RHU module.
// Maps to legacy table names via ToTable("...") and defines keys, FKs, sizes and nullability.
// Review column types (HasColumnType) if you need exact database types.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.RHU.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.RHU
{
    /// <summary>
    /// Tabela ficha1 — contém os lançamentos da ficha financeira.
    /// </summary>
    public class FichaFinanceiraConfiguration : IEntityTypeConfiguration<FichaFinanceira>
    {
        public void Configure(EntityTypeBuilder<FichaFinanceira> b)
        {
            b.ToTable("ficha1");
            b.HasKey(x => new { x.NoMatric, x.CdEmpresa, x.CdFilial, x.CdConta, x.DtConta });

            b.Property(x => x.NoMatric).HasMaxLength(8).IsRequired();
            b.Property(x => x.CdConta).HasMaxLength(4).IsRequired();
            b.Property(x => x.NoProcesso).HasMaxLength(20);

            b.HasOne<Verba>()
             .WithMany()
             .HasForeignKey(x => x.CdConta)
             .HasPrincipalKey(v => v.CdConta)
             .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
