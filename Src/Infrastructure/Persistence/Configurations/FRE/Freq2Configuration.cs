using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>Fluent mapping para Freq2 (tabela freq2).</summary>
    public sealed class Freq2Configuration : IEntityTypeConfiguration<Freq2>
    {
        public void Configure(EntityTypeBuilder<Freq2> b)
        {
            b.ToTable("freq2");
            b.HasKey(e => new { e.Id });
        }
    }
}
