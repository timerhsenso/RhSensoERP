using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Configurations.Security;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("tuse1", schema: "dbo");

        // Chave primária usando cdusuario (não o Guid Id)
        b.HasKey(x => x.CdUsuario);

        // Mapeamento de propriedades
        b.Property(x => x.CdUsuario).HasColumnName("cdusuario").HasMaxLength(30).IsRequired();
        b.Property(x => x.DcUsuario).HasColumnName("dcusuario").HasMaxLength(50).IsRequired();
        b.Property(x => x.SenhaUser).HasColumnName("senhauser").HasMaxLength(20);
        b.Property(x => x.NmImpCche).HasColumnName("nmimpcche").HasMaxLength(50);
        b.Property(x => x.TpUsuario).HasColumnName("tpusuario").HasMaxLength(1).IsRequired();
        b.Property(x => x.NoMatric).HasColumnName("nomatric").HasMaxLength(8);
        b.Property(x => x.CdEmpresa).HasColumnName("cdempresa");
        b.Property(x => x.CdFilial).HasColumnName("cdfilial");
        b.Property(x => x.NoUser).HasColumnName("nouser").IsRequired();
        b.Property(x => x.EmailUsuario).HasColumnName("email_usuario").HasMaxLength(100);
        b.Property(x => x.FlAtivo).HasColumnName("flativo").HasMaxLength(1).IsRequired();
        b.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("newsequentialid()");
        b.Property(x => x.NormalizedUsername).HasColumnName("normalizedusername").HasMaxLength(30);
        b.Property(x => x.IdFuncionario).HasColumnName("idfuncionario");
        b.Property(x => x.FlNaoRecebeEmail).HasColumnName("flnaorecebeemail").HasMaxLength(1);

        // Índice único no Id (Guid)
        b.HasIndex(x => x.Id).IsUnique().HasDatabaseName("UK_tuse1_id");
    }
}