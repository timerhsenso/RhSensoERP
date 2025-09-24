// Auto-generated EF Core configuration for RHU module.
// Maps to legacy table names via ToTable("...") and defines keys, FKs, sizes and nullability.
// Review column types (HasColumnType) if you need exact database types.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.RHU.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.RHU
{
    /// <summary>
    /// Tabela muni1 — contém os municípios (IBGE/UF).
    /// </summary>
    public class MunicipioConfiguration : IEntityTypeConfiguration<Municipio>
    {
        public void Configure(EntityTypeBuilder<Municipio> b)
        {
            b.ToTable("muni1");
            b.HasKey(x => x.CdMunicip);

            b.Property(x => x.CdMunicip).HasMaxLength(5).IsRequired();
            b.Property(x => x.SgEstado).HasMaxLength(2);
            b.Property(x => x.NmMunicip).HasMaxLength(60).IsRequired();
        }
    }
}
