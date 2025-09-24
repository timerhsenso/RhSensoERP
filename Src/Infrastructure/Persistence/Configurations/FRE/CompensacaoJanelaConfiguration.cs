// Auto-generated EF Core configuration for FRE module.
// Maps exactly to legacy table names via ToTable("...") and sets keys, FKs, sizes, and nullability.
// Review column types (HasColumnType) as needed to match your specific RDBMS.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>
    /// Configuração da tabela COMP2 — janelas/intervalos de compensação.
    /// </summary>
    public class CompensacaoJanelaConfiguration : IEntityTypeConfiguration<CompensacaoJanela>
    {
        public void Configure(EntityTypeBuilder<CompensacaoJanela> b)
        {
            b.ToTable("COMP2");
            b.HasKey(x => new { x.IdComp, x.Inicio });

            b.Property(x => x.CdMotOc).HasMaxLength(4).IsRequired();

            b.HasOne(x => x.Compensacao)
             .WithMany(c => c.Janelas)
             .HasForeignKey(x => x.IdComp)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
