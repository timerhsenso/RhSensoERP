using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Core.SEG.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.SEG
{
    /// <summary>
    /// Mapeamento da entidade Sistema para a tabela legacy: tsistema
    /// </summary>
    public sealed class SistemaConfiguration : IEntityTypeConfiguration<Sistema>
    {
        public void Configure(EntityTypeBuilder<Sistema> b)
        {
            // Tabela
            b.ToTable("tsistema");

            // Chave primária (legacy usa CdSistema como PK)
            b.HasKey(x => x.CdSistema);

            // Colunas
            b.Property(x => x.CdSistema)
                .HasColumnName("cdsistema")
                .HasMaxLength(10)
                .IsRequired();

            b.Property(x => x.DcSistema)
                .HasColumnName("dcsistema")
                .HasMaxLength(60)
                .IsRequired();

            b.Property(x => x.Ativo)
                .HasColumnName("ativo")
                .HasDefaultValue(true)
                .IsRequired();

            // Relacionamentos
            // Um Sistema possui muitas Funcoes (FK em Funcao.CdSistema)
            b.HasMany(x => x.Funcoes)
                .WithOne(x => x.Sistema!)
                .HasForeignKey(x => x.CdSistema)
                .OnDelete(DeleteBehavior.Restrict);

            // Ignorar propriedades comuns do BaseEntity que NÃO existem na tabela legacy
            // (ajuste a lista conforme o seu BaseEntity)
            /*
            b.Ignore(x => x.TenantId);
            b.Ignore(x => x.IsDeleted);
            b.Ignore(x => x.CreatedAt);
            b.Ignore(x => x.CreatedBy);
            b.Ignore(x => x.UpdatedAt);
            b.Ignore(x => x.UpdatedBy);
            b.Ignore(x => x.Id);
            b.Ignore(x => x.RowVersion);*/
        }
    }
}
