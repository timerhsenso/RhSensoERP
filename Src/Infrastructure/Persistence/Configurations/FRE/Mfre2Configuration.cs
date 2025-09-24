using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>Fluent mapping para Mfre2 (tabela mfre2).</summary>
    public sealed class Mfre2Configuration : IEntityTypeConfiguration<Mfre2>
    {
        public void Configure(EntityTypeBuilder<Mfre2> b)
        {
            b.ToTable("mfre2");
            b.HasKey(e => new { e.Id });
        }
    }
}
