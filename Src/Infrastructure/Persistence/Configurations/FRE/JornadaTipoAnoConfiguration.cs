// Auto-generated EF Core configuration for FRE module.
// Maps exactly to legacy table names via ToTable("...") and sets keys, FKs, sizes, and nullability.
// Review column types (HasColumnType) as needed to match your specific RDBMS.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>
    /// Configuração da tabela jtpa1 — quantidade de horas por mês por tipo.
    /// </summary>
    public class JornadaTipoAnoConfiguration : IEntityTypeConfiguration<JornadaTipoAno>
    {
        public void Configure(EntityTypeBuilder<JornadaTipoAno> b)
        {
            b.ToTable("jtpa1");
            b.HasKey(x => new { x.CdEmpresa, x.CdFilial, x.TpJornada, x.AaJornada });

            b.Property(x => x.TpJornada).HasMaxLength(1).IsRequired();
            b.Property(x => x.AaJornada).HasMaxLength(4).IsRequired();
        }
    }
}
