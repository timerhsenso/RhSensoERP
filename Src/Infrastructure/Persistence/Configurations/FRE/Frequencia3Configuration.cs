// Auto-generated EF Core configuration for FRE module.
// Maps exactly to legacy table names via ToTable("...") and sets keys, FKs, sizes, and nullability.
// Review column types (HasColumnType) as needed to match your specific RDBMS.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.FRE.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.FRE
{
    /// <summary>
    /// Configuração da tabela freq3 — resumo de fechamento por empresa/filial.
    /// </summary>
    public class Frequencia3Configuration : IEntityTypeConfiguration<Frequencia3>
    {
        public void Configure(EntityTypeBuilder<Frequencia3> b)
        {
            b.ToTable("freq3");
            b.HasKey(x => new { x.CdEmpresa, x.CdFilial, x.DtFrequen });
        }
    }
}
