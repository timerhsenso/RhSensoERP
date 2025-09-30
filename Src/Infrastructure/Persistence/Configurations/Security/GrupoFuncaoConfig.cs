using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.Security;

/// <summary>
/// Configuração EF Core para entidade GrupoFuncao → tabela hbrh1
/// Baseada no script SQL: bd_rhu_copenor.dbo.hbrh1
/// </summary>
public class GrupoFuncaoConfig : IEntityTypeConfiguration<GrupoFuncao>
{
    public void Configure(EntityTypeBuilder<GrupoFuncao> b)
    {
        // Mapear para tabela hbrh1
        b.ToTable("hbrh1");

        // Chave primária: Id (uniqueidentifier)
        b.HasKey(x => x.Id);

        // Mapeamento exato das colunas do banco
        b.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("newsequentialid()") // DEFAULT (newsequentialid())
            .IsRequired();

        b.Property(x => x.CdGrUser)
            .HasColumnName("cdgruser")
            .HasColumnType("varchar(30)")
            .IsRequired();

        b.Property(x => x.CdFuncao)
            .HasColumnName("cdfuncao")
            .HasColumnType("varchar(30)")
            .IsRequired();

        b.Property(x => x.CdAcoes)
            .HasColumnName("cdacoes")
            .HasColumnType("char(20)")
            .IsRequired();

        b.Property(x => x.CdRestric)
            .HasColumnName("cdrestric")
            .HasColumnType("char(1)")
            .IsRequired();

        b.Property(x => x.CdSistema)
            .HasColumnName("cdsistema")
            .HasColumnType("char(10)")
            .IsRequired(false);             // NULL permitido

        b.Property(x => x.IdGrupoDeUsuario)
            .HasColumnName("idgrupodeusuario")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);             // NULL permitido

        // Unique Constraint para Id
        b.HasIndex(x => x.Id)
            .IsUnique()
            .HasDatabaseName("UK_hbrh1_id");

        // Relacionamento com Funcao (FK composta)
        b.HasOne(x => x.Funcao)
            .WithMany(x => x.GrupoFuncoes)
            .HasForeignKey(x => new { x.CdSistema, x.CdFuncao })
            .HasPrincipalKey(x => new { x.CdSistema, x.CdFuncao })
            .HasConstraintName("FK_hbrh1_fucn1_cdsistema_cdfuncao")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false); // Permitir NULL em cdsistema

        // Relacionamento com GrupoDeUsuario (FK por Id)
        b.HasOne(x => x.GrupoDeUsuario)
            .WithMany(x => x.GrupoFuncoes)
            .HasForeignKey(x => x.IdGrupoDeUsuario)
            .HasPrincipalKey(x => x.Id)
            .HasConstraintName("FK_hbrh1_gurh1_idgrupodeusuario")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Constraints já existem no banco:
        // - UK_hbrh1_id
        // - FK_hbrh1_fucn1_cdsistema_cdfuncao
        // - FK_hbrh1_gurh1_idgrupodeusuario
    }
}