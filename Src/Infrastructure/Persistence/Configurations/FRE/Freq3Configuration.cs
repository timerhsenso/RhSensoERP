using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>Fluent mapping para Freq3 (tabela freq3).</summary>
    public sealed class Freq3Configuration : IEntityTypeConfiguration<Freq3>
    {
        public void Configure(EntityTypeBuilder<Freq3> b)
        {
            b.ToTable("freq3");
            b.HasKey(e => new { e.Id });
        }
    }
}
