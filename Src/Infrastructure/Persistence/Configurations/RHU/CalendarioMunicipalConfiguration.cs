// Auto-generated EF Core configuration for RHU module.
// Maps to legacy table names via ToTable("...") and defines keys, FKs, sizes and nullability.
// Review column types (HasColumnType) if you need exact database types.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.RHU.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.RHU
{
    /// <summary>
    /// Tabela calnd1 — contém feriados/ocorrências no calendário municipal.
    /// </summary>
    public class CalendarioMunicipalConfiguration : IEntityTypeConfiguration<CalendarioMunicipal>
    {
        public void Configure(EntityTypeBuilder<CalendarioMunicipal> b)
        {
            b.ToTable("calnd1");
            b.HasKey(x => new { x.CdMunicip, x.DtCalend });

            b.Property(x => x.CdMunicip).HasMaxLength(5).IsRequired();
            b.Property(x => x.CdFeriado).HasMaxLength(1).IsRequired();

            b.HasOne(x => x.Municipio)
             .WithMany(m => m.Calendarios)
             .HasForeignKey(x => x.CdMunicip)
             .HasPrincipalKey(m => m.CdMunicip)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
