// Auto-generated EF Core configuration for RHU module.
// Maps to legacy table names via ToTable("...") and defines keys, FKs, sizes and nullability.
// Review column types (HasColumnType) if you need exact database types.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.RHU.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.RHU
{
    /// <summary>
    /// Tabela tsitu1 — contém as situações usadas em RH.
    /// </summary>
    public class SituacaoConfiguration : IEntityTypeConfiguration<Situacao>
    {
        public void Configure(EntityTypeBuilder<Situacao> b)
        {
            b.ToTable("tsitu1");
            b.HasKey(x => x.CdSituacao);

            b.Property(x => x.CdSituacao).HasMaxLength(2).IsRequired();
            b.Property(x => x.DcSituacao).HasMaxLength(100);
        }
    }
}
