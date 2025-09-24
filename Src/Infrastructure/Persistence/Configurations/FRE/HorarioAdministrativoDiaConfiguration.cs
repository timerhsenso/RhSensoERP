// Auto-generated EF Core configuration for FRE module.
// Maps exactly to legacy table names via ToTable("...") and sets keys, FKs, sizes, and nullability.
// Review column types (HasColumnType) as needed to match your specific RDBMS.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>
    /// Configuração da tabela CHOR2 — dias/turnos do horário administrativo.
    /// </summary>
    public class HorarioAdministrativoDiaConfiguration : IEntityTypeConfiguration<HorarioAdministrativoDia>
    {
        public void Configure(EntityTypeBuilder<HorarioAdministrativoDia> b)
        {
            b.ToTable("CHOR2");
            b.HasKey(x => new { x.CdCargHor, x.DiaDaSemana });

            b.Property(x => x.CdCargHor).HasMaxLength(2).IsRequired();
            b.Property(x => x.HhEntrada).HasMaxLength(5).IsRequired();
            b.Property(x => x.HhSaida).HasMaxLength(5).IsRequired();
            b.Property(x => x.HhIniInt).HasMaxLength(5).IsRequired();
            b.Property(x => x.HhFimInt).HasMaxLength(5).IsRequired();
            b.Property(x => x.CodHors1050).HasMaxLength(20);
            b.Property(x => x.FlHabilitado).IsRequired();

            b.HasOne(x => x.Horario)
             .WithMany(h => h.Dias)
             .HasForeignKey(x => x.CdCargHor)
             .HasPrincipalKey(h => h.CdCargHor)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
