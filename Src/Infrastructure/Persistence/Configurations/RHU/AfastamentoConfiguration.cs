// Auto-generated EF Core configuration for RHU module.
// Maps to legacy table names via ToTable("...") and defines keys, FKs, sizes and nullability.
// Review column types (HasColumnType) if you need exact database types.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.RHU.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.RHU
{
    /// <summary>
    /// Tabela afas1 — contém os afastamentos dos colaboradores.
    /// PK composta: NoMatric, CdEmpresa, CdFilial, DtAfast.
    /// </summary>
    public class AfastamentoConfiguration : IEntityTypeConfiguration<Afastamento>
    {
        public void Configure(EntityTypeBuilder<Afastamento> b)
        {
            b.ToTable("afas1");
            b.HasKey(x => new { x.NoMatric, x.CdEmpresa, x.CdFilial, x.DtAfast });

            b.Property(x => x.NoMatric).HasMaxLength(8).IsRequired();
            b.Property(x => x.CdMotAfas).HasMaxLength(2).IsRequired();
            b.Property(x => x.CdSituacao).HasMaxLength(2).IsRequired();
            b.Property(x => x.CdSitCadas).HasMaxLength(2);

            // FK para moaf1 (motivo) via (CdMotAfas, CdSituacao)
            b.HasOne(x => x.Motivo)
             .WithMany()
             .HasForeignKey(x => new { x.CdMotAfas, x.CdSituacao })
             .HasPrincipalKey(m => new { m.CdMotAfas, m.CdSituacao })
             .OnDelete(DeleteBehavior.NoAction);

            // FK para tsitu1 (situação principal)
            b.HasOne(x => x.Situacao)
             .WithMany()
             .HasForeignKey(x => x.CdSituacao)
             .HasPrincipalKey(s => s.CdSituacao)
             .OnDelete(DeleteBehavior.NoAction);

            // FK para tsitu1 (situação cadastral - opcional)
            b.HasOne(x => x.SituacaoCadastral)
             .WithMany()
             .HasForeignKey(x => x.CdSitCadas)
             .HasPrincipalKey(s => s.CdSituacao)
             .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
