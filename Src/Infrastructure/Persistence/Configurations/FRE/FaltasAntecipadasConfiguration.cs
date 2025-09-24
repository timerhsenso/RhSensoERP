using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>Fluent mapping para FaltasAntecipadas (tabela FALTASANTECIPADAS).</summary>
    public sealed class FaltasAntecipadasConfiguration : IEntityTypeConfiguration<FaltasAntecipadas>
    {
        public void Configure(EntityTypeBuilder<FaltasAntecipadas> b)
        {
            b.ToTable("FALTASANTECIPADAS");
            b.HasKey(e => new { e.Id });
        }
    }
}
