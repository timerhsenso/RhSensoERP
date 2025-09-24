// Auto-generated EF Core configuration for RHU module.
// Maps to legacy table names via ToTable("...") and defines keys, FKs, sizes and nullability.
// Review column types (HasColumnType) if you need exact database types.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.RHU.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.RHU
{
    /// <summary>
    /// Tabela moaf1 — contém os motivos de afastamento e sua situação padrão.
    /// PK composta: CdMotAfas + CdSituacao.
    /// </summary>
    public class MotivoAfastamentoConfiguration : IEntityTypeConfiguration<MotivoAfastamento>
    {
        public void Configure(EntityTypeBuilder<MotivoAfastamento> b)
        {
            b.ToTable("moaf1");
            b.HasKey(x => new { x.CdMotAfas, x.CdSituacao });

            b.Property(x => x.CdMotAfas).HasMaxLength(2).IsRequired();
            b.Property(x => x.CdSituacao).HasMaxLength(2).IsRequired();
            b.Property(x => x.DcMotAfas).HasMaxLength(80);

            b.HasOne(x => x.Situacao)
             .WithMany(s => s.MotivosAfastamento)
             .HasForeignKey(x => x.CdSituacao)
             .HasPrincipalKey(s => s.CdSituacao)
             .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
