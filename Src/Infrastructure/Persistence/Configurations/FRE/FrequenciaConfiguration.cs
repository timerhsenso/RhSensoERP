// Auto-generated EF Core configuration for FRE module.
// Maps exactly to legacy table names via ToTable("...") and sets keys, FKs, sizes, and nullability.
// Review column types (HasColumnType) as needed to match your specific RDBMS.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>
    /// Configuração da tabela freq1 — contém as ocorrências de frequência (faltas, atrasos, HE, abonos).
    /// </summary>
    public class FrequenciaConfiguration : IEntityTypeConfiguration<Frequencia>
    {
        public void Configure(EntityTypeBuilder<Frequencia> b)
        {
            b.ToTable("freq1");
            b.HasKey(x => new { x.NoMatric, x.CdEmpresa, x.CdFilial, x.DtOcorr, x.HhIniOcor, x.TpOcorr });

            b.Property(x => x.NoMatric).HasMaxLength(8).IsRequired();
            b.Property(x => x.CdMotOc).HasMaxLength(4).IsRequired();
            b.Property(x => x.CdCcCusRes).HasMaxLength(20);
            b.Property(x => x.CdUsuario).HasMaxLength(30);
            b.Property(x => x.NoMatTroc).HasMaxLength(8);
            b.Property(x => x.CdUsAprHe).HasMaxLength(30);
            b.Property(x => x.NoProcesso).HasMaxLength(20);
            b.Property(x => x.CdMotOcDefault).HasMaxLength(4);
            b.Property(x => x.CdUsuarioAceito).HasMaxLength(30);
            b.Property(x => x.CdUsuarioAutoriza).HasMaxLength(30);
            b.Property(x => x.CodJustific).HasMaxLength(20);
            b.Property(x => x.TxOcorr).HasMaxLength(1024);

            // FK: motivo (mfre1) via (TpOcorr, CdMotOc)
            b.HasOne(x => x.Motivo)
             .WithMany() // sem navegação inversa para evitar incompatibilidade de tipos
             .HasForeignKey(x => new { x.TpOcorr, x.CdMotOc })
             .HasPrincipalKey(m => new { m.TpOcorr, m.CdMotOc })
             .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
