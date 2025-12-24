using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Core.Entities;

namespace RhSensoERP.Identity.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento EF Core da entidade <see cref="Usuario"/> para a tabela dbo.tuse1.
/// - PK: cdusuario (varchar(30))
/// - Alternate Key: id (uniqueidentifier)
/// - Colunas e tamanhos conforme legado
/// </summary>
public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("tuse1");

        // ===== Chaves =====
        builder.HasKey(e => e.CdUsuario);
        builder.HasAlternateKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        // ===== Colunas principais =====
        builder.Property(e => e.CdUsuario)
            .HasColumnName("cdusuario")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.DcUsuario)
            .HasColumnName("dcusuario")
            .HasMaxLength(50)
            .IsRequired();

        // ===== Credenciais / normalização =====
        builder.Property(e => e.SenhaUser)
            .HasColumnName("senhauser")
            .HasMaxLength(20);

        builder.Property(e => e.PasswordHash)
            .HasColumnName("PasswordHash")
            .HasMaxLength(255);

        builder.Property(e => e.PasswordSalt)
            .HasColumnName("PasswordSalt")
            .HasMaxLength(255);

        builder.Property(e => e.NormalizedUserName)
            .HasColumnName("normalizedusername")
            .HasMaxLength(30);

        // ===== Dados funcionais / empresa =====
        builder.Property(e => e.NmImpcche)
            .HasColumnName("nmimpcche")
            .HasMaxLength(50);

        builder.Property(e => e.TpUsuario)
            .HasColumnName("tpusuario")
            .HasColumnType("char(1)");

        builder.Property(e => e.NoMatric)
            .HasColumnName("nomatric")
            .HasMaxLength(8)
            .IsFixedLength();

        builder.Property(e => e.CdEmpresa)
            .HasColumnName("cdempresa");

        builder.Property(e => e.CdFilial)
            .HasColumnName("cdfilial");

        builder.Property(e => e.NoUser)
            .HasColumnName("nouser")
            .IsRequired();

        builder.Property(e => e.Email_Usuario)
            .HasColumnName("email_usuario")
            .HasMaxLength(100);

        builder.Property(e => e.FlAtivo)
            .HasColumnName("flativo")
            .HasColumnType("char(1)")
            .IsRequired();

        builder.Property(e => e.FlNaoRecebeEmail)
            .HasColumnName("flnaorecebeemail")
            .HasColumnType("char(1)");

        builder.Property(e => e.IdFuncionario)
            .HasColumnName("idfuncionario");

        // ===== Multi-tenant =====
        builder.Property(e => e.TenantPrincipal)
            .HasColumnName("TenantPrincipal");

        builder.Property(e => e.TenantId)
            .HasColumnName("TenantId");

        // ===== Autenticação / confirmação e-mail =====
        builder.Property(e => e.AuthMode)
            .HasColumnName("AuthMode")
            .HasMaxLength(20)
            .HasDefaultValue("OnPrem")
            .IsRequired();

        builder.Property(e => e.EmailConfirmed)
            .HasColumnName("EmailConfirmed")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(e => e.EmailConfirmationToken)
            .HasColumnName("EmailConfirmationToken")
            .HasMaxLength(255);

        builder.Property(e => e.EmailConfirmationTokenExpiry)
            .HasColumnName("EmailConfirmationTokenExpiry");

        // ===== Reset de senha =====
        builder.Property(e => e.PasswordResetToken)
            .HasColumnName("PasswordResetToken")
            .HasMaxLength(255);

        builder.Property(e => e.PasswordResetTokenExpiry)
            .HasColumnName("PasswordResetTokenExpiry");

        builder.Property(e => e.PasswordResetRequestedAt)
            .HasColumnName("PasswordResetRequestedAt");

        builder.Property(e => e.PasswordResetRequestedBy)
            .HasColumnName("PasswordResetRequestedBy")
            .HasMaxLength(30);

        builder.Property(e => e.LoginAttempts)
            .HasColumnName("LoginAttempts")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(e => e.LockedUntil)
            .HasColumnName("LockedUntil");

        builder.Property(e => e.LastFailedLoginAt)
            .HasColumnName("LastFailedLoginAt");

        builder.Property(e => e.LastLoginAt)
            .HasColumnName("LastLoginAt");

        builder.Property(e => e.LastPasswordChangedAt)
            .HasColumnName("LastPasswordChangedAt");

        // ===== 2FA =====
        builder.Property(e => e.TwoFactorEnabled)
            .HasColumnName("TwoFactorEnabled")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(e => e.TwoFactorSecret)
            .HasColumnName("TwoFactorSecret")
            .HasMaxLength(255);

        builder.Property(e => e.TwoFactorBackupCodes)
            .HasColumnName("TwoFactorBackupCodes"); // nvarchar(max)

        // ===== Auditoria / rede =====
        builder.Property(e => e.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasDefaultValueSql("getutcdate()")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasDefaultValueSql("getutcdate()")
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasMaxLength(30);

        builder.Property(e => e.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasMaxLength(30);

        builder.Property(e => e.LastUserAgent)
            .HasColumnName("LastUserAgent")
            .HasMaxLength(500);

        builder.Property(e => e.LastIpAddress)
            .HasColumnName("LastIpAddress")
            .HasMaxLength(45);

        // ===== Índices úteis =====
        builder.HasIndex(e => e.Id).IsUnique();
        builder.HasIndex(e => e.Email_Usuario);
        builder.HasIndex(e => e.NormalizedUserName);
    }
}
