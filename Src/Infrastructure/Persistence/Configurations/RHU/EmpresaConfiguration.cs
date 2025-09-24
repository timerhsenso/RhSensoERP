// Auto-generated EF Core configuration for RHU module.
// Maps to legacy table names via ToTable("...") and defines keys, FKs, sizes and nullability.
// Review column types (HasColumnType) if you need exact database types.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.RHU.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.RHU
{
    /// <summary>
    /// Tabela temp1 — contém as empresas.
    /// </summary>
    public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> b)
        {
            b.ToTable("temp1");
            b.HasKey(x => x.CdEmpresa);

            b.Property(x => x.NmEmpresa).HasMaxLength(200);
            b.Property(x => x.NmFantasia).HasMaxLength(200);
            b.Property(x => x.NmArqLogo).HasMaxLength(255);
            b.Property(x => x.NmArqLogoCracha).HasMaxLength(255);
            b.Property(x => x.TpInscEmpregador).HasMaxLength(2);
            b.Property(x => x.NrInscEmpregador).HasMaxLength(20);
            b.Property(x => x.FlAtivo).HasMaxLength(1);
            b.Property(x => x.ArquivoLogo).HasMaxLength(255);
            b.Property(x => x.ArquivoLogoCracha).HasMaxLength(255);
            b.Property(x => x.Classtrib).HasMaxLength(10);
            b.Property(x => x.CnpjEfr).HasMaxLength(14);
            b.Property(x => x.IdEmInLei).HasMaxLength(10);
            b.Property(x => x.IndPorte).HasMaxLength(1);
            b.Property(x => x.NatJuridica).HasMaxLength(10);
            b.Property(x => x.NrCertificado).HasMaxLength(100);
            b.Property(x => x.NrProtRenovacao).HasMaxLength(100);
            b.Property(x => x.NrRegTt).HasMaxLength(50);
            b.Property(x => x.PaginaDou).HasMaxLength(20);
        }
    }
}
