using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>Fluent mapping para Batidas (tabela BATIDAS).</summary>
    public sealed class BatidasConfiguration : IEntityTypeConfiguration<Batidas>
    {
        public void Configure(EntityTypeBuilder<Batidas> b)
        {
            b.ToTable("BATIDAS");
            b.HasKey(e => new { e.CdEmpresa, e.CdFilial, e.NoMatric, e.Data, e.Hora });
        }
    }
}
