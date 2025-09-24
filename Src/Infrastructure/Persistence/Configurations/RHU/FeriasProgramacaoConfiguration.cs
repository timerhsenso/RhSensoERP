// Auto-generated EF Core configuration for RHU module.
// Maps to legacy table names via ToTable("...") and defines keys, FKs, sizes and nullability.
// Review column types (HasColumnType) if you need exact database types.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.RHU.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.RHU
{
    /// <summary>
    /// Tabela feria2 — contém as programações de férias dos colaboradores.
    /// PK composta: NoMatric, CdEmpresa, CdFilial, DtIniPa, NoSequenc.
    /// </summary>
    public class FeriasProgramacaoConfiguration : IEntityTypeConfiguration<FeriasProgramacao>
    {
        public void Configure(EntityTypeBuilder<FeriasProgramacao> b)
        {
            b.ToTable("feria2");
            b.HasKey(x => new { x.NoMatric, x.CdEmpresa, x.CdFilial, x.DtIniPa, x.NoSequenc });

            b.Property(x => x.NoMatric).HasMaxLength(8).IsRequired();
            b.Property(x => x.FlStatusAprovaAviso).HasMaxLength(2);
            b.Property(x => x.CdUserAprovaAviso).HasMaxLength(30);
            b.Property(x => x.QuemAlterou).HasMaxLength(60);
        }
    }
}
