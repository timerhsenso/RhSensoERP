using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>Fluent mapping para Chor2 (tabela CHOR2).</summary>
    public sealed class Chor2Configuration : IEntityTypeConfiguration<Chor2>
    {
        public void Configure(EntityTypeBuilder<Chor2> b)
        {
            b.ToTable("CHOR2");
            b.HasKey(e => new { e.CdCargHor, e.DiaDaSemana });
        }
    }
}
