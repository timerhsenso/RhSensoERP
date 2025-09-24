// Auto-generated EF Core configuration for FRE module.
// Maps exactly to legacy table names via ToTable("...") and sets keys, FKs, sizes, and nullability.
// Review column types (HasColumnType) as needed to match your specific RDBMS.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>
    /// Configuração da tabela ctur1 — calendário/escala de turmas.
    /// </summary>
    public class TurmaCalendarioConfiguration : IEntityTypeConfiguration<TurmaCalendario>
    {
        public void Configure(EntityTypeBuilder<TurmaCalendario> b)
        {
            b.ToTable("ctur1");
            b.HasKey(x => new { x.CdEmpresa, x.CdFilial, x.CdTurma, x.DtCalend });

            b.Property(x => x.CdTurma).HasMaxLength(2).IsRequired();
            b.Property(x => x.HhEntrada).HasMaxLength(5);
            b.Property(x => x.HhSaida).HasMaxLength(5);
            b.Property(x => x.PontoRepeticao).HasMaxLength(30);
        }
    }
}
