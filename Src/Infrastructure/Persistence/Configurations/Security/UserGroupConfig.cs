using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.Security;

/// <summary>
/// Configuração EF Core para entidade UserGroup → tabela usrh1
/// Baseada no script SQL: bd_rhu_copenor.dbo.usrh1
/// </summary>
public class UserGroupConfig : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> b)
    {
        // Mapear para tabela usrh1
        b.ToTable("usrh1", schema: "dbo");

        // Chave primária: Id (uniqueidentifier)
        b.HasKey(x => x.Id);

        // Mapeamento exato das colunas do banco
        b.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("newsequentialid()") // DEFAULT (newsequentialid())
            .IsRequired();

        b.Property(x => x.CdUsuario)
            .HasColumnName("cdusuario")
            .HasColumnType("varchar(30)")
            .IsRequired();

        b.Property(x => x.CdGrUser)
            .HasColumnName("cdgruser")
            .HasColumnType("varchar(30)")
            .IsRequired();

        b.Property(x => x.DtIniVal)
            .HasColumnName("dtinival")
            .HasColumnType("datetime")
            .IsRequired();

        b.Property(x => x.DtFimVal)
            .HasColumnName("dtfimval")
            .HasColumnType("datetime")
            .IsRequired(false);             // NULL permitido

        b.Property(x => x.CdSistema)
            .HasColumnName("cdsistema")
            .HasColumnType("char(10)")
            .IsRequired(false);             // NULL permitido

        b.Property(x => x.IdUsuario)
            .HasColumnName("idusuario")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);             // NULL permitido

        b.Property(x => x.IdGrupoDeUsuario)
            .HasColumnName("idgrupodeusuario")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);             // NULL permitido

        // Unique Constraints do banco
        b.HasIndex(x => x.Id)
            .IsUnique()
            .HasDatabaseName("UK_usrh1_id");

        b.HasIndex(x => new { x.CdUsuario, x.CdSistema, x.CdGrUser, x.DtIniVal })
            .IsUnique()
            .HasDatabaseName("UK_usrh1_cdusuario_cdsistema_cdgruser_dtinival");

        // Relacionamento com User (FK por Id)
        b.HasOne(x => x.User)
            .WithMany(x => x.UserGroups)
            .HasForeignKey(x => x.IdUsuario)
            .HasPrincipalKey(x => x.Id)
            .HasConstraintName("FK_usrh1_tuse1_idusuario")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Relacionamento com GrupoDeUsuario (gurh1) - configuraremos quando tiver a entidade
        // b.HasOne(x => x.GrupoDeUsuario)
        //     .WithMany()
        //     .HasForeignKey(x => x.IdGrupoDeUsuario)
        //     .HasConstraintName("FK_usrh1_gurh1_idgrupodeusuario")
        //     .OnDelete(DeleteBehavior.Restrict)
        //     .IsRequired(false);

        // Constraints já existem no banco:
        // - UK_usrh1_id
        // - UK_usrh1_cdusuario_cdsistema_cdgruser_dtinival
        // - FK_usrh1_tuse1_idusuario
        // - FK_usrh1_gurh1_idgrupodeusuario
    }
}