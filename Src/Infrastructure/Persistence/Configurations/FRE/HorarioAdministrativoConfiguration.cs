// Auto-generated EF Core configuration for FRE module.
// Maps exactly to legacy table names via ToTable("...") and sets keys, FKs, sizes, and nullability.
// Review column types (HasColumnType) as needed to match your specific RDBMS.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>
    /// Configuração da tabela chor1 — horários administrativos (grade base).
    /// </summary>
    public class HorarioAdministrativoConfiguration : IEntityTypeConfiguration<HorarioAdministrativo>
    {
        public void Configure(EntityTypeBuilder<HorarioAdministrativo> b)
        {
            b.ToTable("chor1");
            b.HasKey(x => x.CdCargHor);

            b.Property(x => x.CdCargHor).HasMaxLength(2).IsRequired();
            b.Property(x => x.HhEntrada).HasMaxLength(5).IsRequired();
            b.Property(x => x.HhSaida).HasMaxLength(5).IsRequired();
            b.Property(x => x.HhIniInt).HasMaxLength(5).IsRequired();
            b.Property(x => x.HhFimInt).HasMaxLength(5).IsRequired();
            b.Property(x => x.DcCargHor).HasMaxLength(100).IsRequired();
            b.Property(x => x.FlIntervalo).HasMaxLength(1);
            b.Property(x => x.CodHors1050).HasMaxLength(20);
        }
    }
}
