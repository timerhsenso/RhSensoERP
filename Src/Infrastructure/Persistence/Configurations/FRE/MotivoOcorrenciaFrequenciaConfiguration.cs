// Auto-generated EF Core configuration for FRE module.
// Maps exactly to legacy table names via ToTable("...") and sets keys, FKs, sizes, and nullability.
// Review column types (HasColumnType) as needed to match your specific RDBMS.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>
    /// Configuração da tabela mfre1 — motivos de ocorrência de frequência.
    /// </summary>
    public class MotivoOcorrenciaFrequenciaConfiguration : IEntityTypeConfiguration<MotivoOcorrenciaFrequencia>
    {
        public void Configure(EntityTypeBuilder<MotivoOcorrenciaFrequencia> b)
        {
            b.ToTable("mfre1");
            b.HasKey(x => new { x.TpOcorr, x.CdMotOc });

            b.Property(x => x.CdMotOc).HasMaxLength(4).IsRequired();
            b.Property(x => x.DcMotOc).HasMaxLength(40);
            b.Property(x => x.CdConta).HasMaxLength(4);
            b.Property(x => x.CdMotOcLink).HasMaxLength(4);
        }
    }
}
