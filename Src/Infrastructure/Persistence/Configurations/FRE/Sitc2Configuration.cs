using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>Fluent mapping para Sitc2 (tabela sitc2).</summary>
    public sealed class Sitc2Configuration : IEntityTypeConfiguration<Sitc2>
    {
        public void Configure(EntityTypeBuilder<Sitc2> b)
        {
            b.ToTable("sitc2");
            b.HasKey(e => new { e.Id });
        }
    }
}
