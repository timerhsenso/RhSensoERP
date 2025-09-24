using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>Fluent mapping para Freq4 (tabela FREQ4).</summary>
    public sealed class Freq4Configuration : IEntityTypeConfiguration<Freq4>
    {
        public void Configure(EntityTypeBuilder<Freq4> b)
        {
            b.ToTable("FREQ4");
            b.HasKey(e => new { e.NoMatric, e.CdEmpresa, e.CdFilial, e.Data });
        }
    }
}
