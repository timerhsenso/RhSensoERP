using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserGroupConfig : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> b)
    {
        b.ToTable("usrh1", schema: "dbo");

        b.HasKey(x => x.Id);

        b.Property(x => x.CdUsuario).HasColumnName("cdusuario").HasMaxLength(30).IsRequired();
        b.Property(x => x.CdGrUser).HasColumnName("cdgruser").HasMaxLength(30).IsRequired();
        b.Property(x => x.DtIniVal).HasColumnName("dtinival").IsRequired();
        b.Property(x => x.DtFimVal).HasColumnName("dtfimval");
        b.Property(x => x.CdSistema).HasColumnName("cdsistema").HasMaxLength(10);
        b.Property(x => x.IdUsuario).HasColumnName("idusuario");
        b.Property(x => x.IdGrupoDeUsuario).HasColumnName("idgrupodeusuario");
        b.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("newsequentialid()");

        // Relacionamento com User
        b.HasOne(x => x.User)
         .WithMany(x => x.UserGroups)
         .HasForeignKey(x => x.CdUsuario)
         .HasPrincipalKey(x => x.CdUsuario);

        // Índice único existente
        b.HasIndex(x => new { x.CdUsuario, x.CdSistema, x.CdGrUser, x.DtIniVal })
         .IsUnique()
         .HasDatabaseName("UK_usrh1_cdusuario_cdsistema_cdgruser_dtinival");
    }
}