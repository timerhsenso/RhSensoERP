using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>Fluent mapping para Comp1 (tabela COMP1).</summary>
    public sealed class Comp1Configuration : IEntityTypeConfiguration<Comp1>
    {
        public void Configure(EntityTypeBuilder<Comp1> b)
        {
            b.ToTable("COMP1");
            b.HasKey(e => new { e.Id });
        }
    }
}
