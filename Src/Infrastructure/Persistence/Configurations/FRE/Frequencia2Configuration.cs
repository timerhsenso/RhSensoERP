// Auto-generated EF Core configuration for FRE module.
// Maps exactly to legacy table names via ToTable("...") and sets keys, FKs, sizes, and nullability.
// Review column types (HasColumnType) as needed to match your specific RDBMS.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>
    /// Configuração da tabela freq2 — registros compactos/resumo por colaborador/dia.
    /// </summary>
    public class Frequencia2Configuration : IEntityTypeConfiguration<Frequencia2>
    {
        public void Configure(EntityTypeBuilder<Frequencia2> b)
        {
            b.ToTable("freq2");
            b.HasKey(x => x.Id);

            b.Property(x => x.NoMatric).HasMaxLength(8).IsRequired();
        }
    }
}
