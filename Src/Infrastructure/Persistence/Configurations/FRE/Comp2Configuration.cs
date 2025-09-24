using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>Fluent mapping para Comp2 (tabela COMP2).</summary>
    public sealed class Comp2Configuration : IEntityTypeConfiguration<Comp2>
    {
        public void Configure(EntityTypeBuilder<Comp2> b)
        {
            b.ToTable("COMP2");
            b.HasKey(e => new { e.IdComp, e.Inicio });
        }
    }
}
