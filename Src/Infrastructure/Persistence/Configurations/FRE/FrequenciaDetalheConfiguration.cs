// Auto-generated EF Core configuration for FRE module.
// Maps exactly to legacy table names via ToTable("...") and sets keys, FKs, sizes, and nullability.
// Review column types (HasColumnType) as needed to match your specific RDBMS.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>
    /// Configuração da tabela FREQ4 — detalhamento de marcações/intervalos.
    /// </summary>
    public class FrequenciaDetalheConfiguration : IEntityTypeConfiguration<FrequenciaDetalhe>
    {
        public void Configure(EntityTypeBuilder<FrequenciaDetalhe> b)
        {
            b.ToTable("FREQ4");
            b.HasKey(x => new { x.NoMatric, x.CdEmpresa, x.CdFilial, x.Data, x.Inicio });
            b.Property(x => x.NoMatric).HasMaxLength(8).IsRequired();
            b.Property(x => x.FlIntervalo).HasMaxLength(1);
        }
    }
}
