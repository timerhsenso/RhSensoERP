using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>Fluent mapping para Hjor1 (tabela hjor1).</summary>
    public sealed class Hjor1Configuration : IEntityTypeConfiguration<Hjor1>
    {
        public void Configure(EntityTypeBuilder<Hjor1> b)
        {
            b.ToTable("hjor1");
            b.HasKey(e => new { e.NoMatric, e.CdEmpresa, e.CdFilial, e.DtMudanca });
        }
    }
}
