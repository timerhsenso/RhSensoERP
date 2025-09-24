// Auto-generated EF Core configuration for RHU module.
// Maps to legacy table names via ToTable("...") and defines keys, FKs, sizes and nullability.
// Review column types (HasColumnType) if you need exact database types.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.RHU.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.RHU
{
    /// <summary>
    /// Tabela calc1 — contém lançamentos calculados por processo de folha.
    /// PK composta: NoMatric, CdEmpresa, CdFilial, NoProcesso, CdConta.
    /// </summary>
    public class LancamentoCalculadoConfiguration : IEntityTypeConfiguration<LancamentoCalculado>
    {
        public void Configure(EntityTypeBuilder<LancamentoCalculado> b)
        {
            b.ToTable("calc1");
            b.HasKey(x => new { x.NoMatric, x.CdEmpresa, x.CdFilial, x.NoProcesso, x.CdConta });

            b.Property(x => x.NoMatric).HasMaxLength(8).IsRequired();
            b.Property(x => x.CdConta).HasMaxLength(4).IsRequired();
            b.Property(x => x.NoProcesso).HasMaxLength(6).IsRequired();
            b.Property(x => x.CdUsuario).HasMaxLength(30);

            b.HasOne<Verba>()
             .WithMany()
             .HasForeignKey(x => x.CdConta)
             .HasPrincipalKey(v => v.CdConta)
             .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
