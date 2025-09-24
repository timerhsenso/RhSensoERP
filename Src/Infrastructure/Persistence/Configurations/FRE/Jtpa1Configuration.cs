using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>Fluent mapping para Jtpa1 (tabela jtpa1).</summary>
    public sealed class Jtpa1Configuration : IEntityTypeConfiguration<Jtpa1>
    {
        public void Configure(EntityTypeBuilder<Jtpa1> b)
        {
            b.ToTable("jtpa1");
            b.HasKey(e => new { e.CdEmpresa, e.CdFilial, e.TpJornada, e.AaJornada });
        }
    }
}
