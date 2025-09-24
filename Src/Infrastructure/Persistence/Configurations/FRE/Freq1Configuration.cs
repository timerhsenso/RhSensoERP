using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>Fluent mapping para Freq1 (tabela freq1).</summary>
    public sealed class Freq1Configuration : IEntityTypeConfiguration<Freq1>
    {
        public void Configure(EntityTypeBuilder<Freq1> b)
        {
            b.ToTable("freq1");
            b.HasKey(e => new { e.NoMatric, e.CdEmpresa, e.CdFilial, e.DtOcorr, e.HhIniOcor, e.TpOcorr });
        }
    }
}
