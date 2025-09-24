// Auto-generated EF Core configuration for FRE module.
// Maps exactly to legacy table names via ToTable("...") and sets keys, FKs, sizes, and nullability.
// Review column types (HasColumnType) as needed to match your specific RDBMS.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>
    /// Configuração da tabela mfre2 — escopo por empresa/filial dos motivos de ocorrência.
    /// </summary>
    public class MotivoOcorrenciaFrequenciaEmpresaConfiguration : IEntityTypeConfiguration<MotivoOcorrenciaFrequenciaEmpresa>
    {
        public void Configure(EntityTypeBuilder<MotivoOcorrenciaFrequenciaEmpresa> b)
        {
            b.ToTable("mfre2");
            b.HasKey(x => new { x.CdEmpresa, x.CdFilial, x.TpOcorr, x.CdMotOc });

            b.Property(x => x.CdMotOc).HasMaxLength(4).IsRequired();

            b.HasOne(x => x.Motivo)
             .WithMany(m => m.EscoposEmpresa)
             .HasForeignKey(x => new { x.TpOcorr, x.CdMotOc })
             .HasPrincipalKey(m => new { m.TpOcorr, m.CdMotOc })
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
