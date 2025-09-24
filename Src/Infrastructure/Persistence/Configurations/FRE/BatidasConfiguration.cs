// Auto-generated EF Core configuration for FRE module.
// Maps exactly to legacy table names via ToTable("...") and sets keys, FKs, sizes, and nullability.
// Review column types (HasColumnType) as needed to match your specific RDBMS.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>
    /// Configuração da tabela BATIDAS — contém as marcações de ponto brutas (coletores).
    /// </summary>
    public class BatidasConfiguration : IEntityTypeConfiguration<Batida>
    {
        public void Configure(EntityTypeBuilder<Batida> b)
        {
            b.ToTable("BATIDAS");
            b.HasKey(x => x.Id); // observe: PK de negócio é composta, mas o schema possui Id

            b.Property(x => x.NoMatric).HasMaxLength(8).IsRequired();
            b.Property(x => x.Hora).HasMaxLength(5).IsRequired();
            b.Property(x => x.Tipo).HasMaxLength(2).IsRequired();
            b.Property(x => x.Erro).HasMaxLength(10).IsRequired();
            b.Property(x => x.Motivo).HasMaxLength(200);
        }
    }
}
