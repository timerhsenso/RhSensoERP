// Auto-generated EF Core configuration for RHU module.
// Maps to legacy table names via ToTable("...") and defines keys, FKs, sizes and nullability.
// Review column types (HasColumnType) if you need exact database types.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.RHU.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.RHU
{
    /// <summary>
    /// Tabela cargo1 — contém os cargos/funções.
    /// </summary>
    public class CargoConfiguration : IEntityTypeConfiguration<Cargo>
    {
        public void Configure(EntityTypeBuilder<Cargo> b)
        {
            b.ToTable("cargo1");
            b.HasKey(x => x.CdCargo);

            b.Property(x => x.CdCargo).HasMaxLength(5).IsRequired();
            b.Property(x => x.DcCargo).HasMaxLength(50);
            b.Property(x => x.CdInstruc).HasMaxLength(2).IsRequired();
            b.Property(x => x.CdCbo).HasMaxLength(5);
            b.Property(x => x.CdTabela).HasMaxLength(3);
            b.Property(x => x.CdGrProf).HasMaxLength(2);
            b.Property(x => x.CdCbo6).HasMaxLength(6);
        }
    }
}
