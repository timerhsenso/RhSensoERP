using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>Fluent mapping para Chor1 (tabela chor1).</summary>
    public sealed class Chor1Configuration : IEntityTypeConfiguration<Chor1>
    {
        public void Configure(EntityTypeBuilder<Chor1> b)
        {
            b.ToTable("chor1");
            b.HasKey(e => new { e.Id });
        }
    }
}
