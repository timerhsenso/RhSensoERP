using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>Fluent mapping para Jornada (tabela jornada).</summary>
    public sealed class JornadaConfiguration : IEntityTypeConfiguration<Jornada>
    {
        public void Configure(EntityTypeBuilder<Jornada> b)
        {
            b.ToTable("jornada");
            b.HasKey(e => new { e.CdEmpresa, e.CdFilial, e.TpJornada, e.Ano, e.Mes });
        }
    }
}
