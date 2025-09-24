using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>Fluent mapping para Mfre1 (tabela mfre1).</summary>
    public sealed class Mfre1Configuration : IEntityTypeConfiguration<Mfre1>
    {
        public void Configure(EntityTypeBuilder<Mfre1> b)
        {
            b.ToTable("mfre1");
            b.HasKey(e => new { e.TpOcorr, e.CdMotoc });
        }
    }
}
