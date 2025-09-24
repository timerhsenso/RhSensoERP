// Auto-generated EF Core configuration for FRE module.
// Maps exactly to legacy table names via ToTable("...") and sets keys, FKs, sizes, and nullability.
// Review column types (HasColumnType) as needed to match your specific RDBMS.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;
using RhSensoERP.Core.RHU.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>
    /// Configuração da tabela BancoHoras — contém as movimentações de banco de horas dos colaboradores.
    /// </summary>
    public class BancoHorasConfiguration : IEntityTypeConfiguration<BancoHoras>
    {
        public void Configure(EntityTypeBuilder<BancoHoras> b)
        {
            b.ToTable("BancoHoras");
            b.HasKey(x => x.Id);

            b.Property(x => x.NoMatric).HasMaxLength(8).IsRequired();
            b.Property(x => x.DebCred).HasMaxLength(1).IsRequired();
            b.Property(x => x.Tipo).HasMaxLength(1).IsRequired();
            b.Property(x => x.Descricao).HasMaxLength(100);
            b.Property(x => x.CdConta).HasMaxLength(4);

            // FK opcional para tcon2 (RHU.Verba) via CdConta
            b.HasOne<Verba>(x => x.Conta)
             .WithMany()
             .HasForeignKey(x => x.CdConta)
             .HasPrincipalKey(v => v.CdConta)
             .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
