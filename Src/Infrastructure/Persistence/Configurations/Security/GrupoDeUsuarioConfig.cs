using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.Security;

/// <summary>
/// Configuração EF Core para entidade GrupoDeUsuario → tabela gurh1
/// Baseada no script SQL: bd_rhu_copenor.dbo.gurh1
/// </summary>
public class GrupoDeUsuarioConfig : IEntityTypeConfiguration<GrupoDeUsuario>
{
    public void Configure(EntityTypeBuilder<GrupoDeUsuario> b)
    {
        // Mapear para tabela gurh1
        b.ToTable("gurh1");

        // Chave primária COMPOSTA: (cdsistema, cdgruser)
        b.HasKey(x => new { x.CdSistema, x.CdGrUser });

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

        b.Property(x => x.DcGrUser)
            .HasColumnName("dcgruser")
            .HasColumnType("varchar(60)")
            .IsRequired(false);             // NULL permitido

        b.Property(x => x.CdSistema)
            .HasColumnName("cdsistema")
            .HasColumnType("char(10)")
            .IsRequired();

        // Unique Constraint para Id
        b.HasIndex(x => x.Id)
            .IsUnique()
            .HasDatabaseName("UK_gurh1_id");

        // Relacionamento com Sistema (FK)
        b.HasOne(x => x.Sistema)
            .WithMany() // Sistema não tem navegação reversa para GrupoDeUsuario
            .HasForeignKey(x => x.CdSistema)
            .HasPrincipalKey(x => x.CdSistema)
            .HasConstraintName("FK_gurh1_tsistema_cdsistema")
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento com UserGroups
        b.HasMany(x => x.UserGroups)
            .WithOne() // UserGroup já tem propriedade User
            .HasForeignKey(x => x.IdGrupoDeUsuario)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento com GrupoFuncoes (configuraremos quando fizer hbrh1)
        b.HasMany(x => x.GrupoFuncoes)
            .WithOne() // Sem navegação reversa por enquanto
            .HasForeignKey(x => x.IdGrupoDeUsuario)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);

        // Constraints já existem no banco:
        // - PK_gurh1 (cdsistema, cdgruser)
        // - UK_gurh1_id
        // - FK_gurh1_tsistema_cdsistema
    }
}