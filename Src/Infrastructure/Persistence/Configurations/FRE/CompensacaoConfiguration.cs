// Auto-generated EF Core configuration for FRE module.
// Maps exactly to legacy table names via ToTable("...") and sets keys, FKs, sizes, and nullability.
// Review column types (HasColumnType) as needed to match your specific RDBMS.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>
    /// Configuração da tabela COMP1 — configuração de compensações (escopo).
    /// </summary>
    public class CompensacaoConfiguration : IEntityTypeConfiguration<Compensacao>
    {
        public void Configure(EntityTypeBuilder<Compensacao> b)
        {
            b.ToTable("COMP1");
            b.HasKey(x => x.Id);

            b.Property(x => x.CdCcusto).HasMaxLength(5);
            b.Property(x => x.Motivo).HasMaxLength(150).IsRequired();
            b.Property(x => x.TpJornada).HasMaxLength(1);
            b.Property(x => x.CdTurma).HasMaxLength(2);
            b.Property(x => x.CdCargHor).HasMaxLength(2);
        }
    }
}
