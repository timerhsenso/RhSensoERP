// Auto-generated EF Core configuration for RHU module.
// Maps to legacy table names via ToTable("...") and defines keys, FKs, sizes and nullability.
// Review column types (HasColumnType) if you need exact database types.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.RHU.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.RHU
{
    /// <summary>
    /// Tabela test1 — contém as filiais/estabelecimentos.
    /// </summary>
    public class FilialConfiguration : IEntityTypeConfiguration<Filial>
    {
        public void Configure(EntityTypeBuilder<Filial> b)
        {
            b.ToTable("test1");
            b.HasKey(x => new { x.CdEmpresa, x.CdFilial });

            b.Property(x => x.NmFantasia).HasMaxLength(200);
            b.Property(x => x.DcEstab).HasMaxLength(200);
            b.Property(x => x.DcEndereco).HasMaxLength(200);
            b.Property(x => x.DcBairro).HasMaxLength(100);
            b.Property(x => x.SgEstado).HasMaxLength(2);
            b.Property(x => x.NoCep).HasMaxLength(10);
            b.Property(x => x.NoTelefone).HasMaxLength(20);
            b.Property(x => x.NoFax).HasMaxLength(20);

            b.HasOne(x => x.Empresa)
             .WithMany(e => e.Filiais)
             .HasForeignKey(x => x.CdEmpresa)
             .HasPrincipalKey(e => e.CdEmpresa)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
