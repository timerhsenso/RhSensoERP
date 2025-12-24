// ============================================================================
//  RhSensoERP.Identity.Domain.Entities.Usuario
//  Tabela: dbo.tuse1
//  Autor: Carlos Eduardo
//  Data: 06/11/2025
//  Descrição: Entidade de usuários do sistema, compatível com a tabela real.
// ============================================================================

using RhSensoERP.Shared.Core.Primitives;
using System;
using System.Collections.Generic;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Identity.Core.Entities
{
    /// <summary>
    /// Representa um usuário do sistema (tabela dbo.tuse1).
    /// Inclui autenticação, auditoria, 2FA e suporte multi-tenant.
    /// </summary>
    public class Usuario
    {
        // =======================
        //  Identificação
        // =======================

        /// <summary>Chave primária lógica (legado) — cdusuario (varchar(30)).</summary>
        public string CdUsuario { get; set; } = string.Empty;

        /// <summary>Chave única alternativa — id (uniqueidentifier).</summary>
        public Guid Id { get; set; }

        /// <summary>Descrição/Nome do usuário — dcusuario (varchar(50)).</summary>
        public string DcUsuario { get; set; } = string.Empty;

        // =======================
        //  Credenciais
        // =======================

        public string? SenhaUser { get; set; }
        public string? PasswordHash { get; set; }
        public string? PasswordSalt { get; set; }
        public string? NormalizedUserName { get; set; }

        // =======================
        //  Dados funcionais
        // =======================

        public string? NmImpcche { get; set; }
        public char TpUsuario { get; set; }
        public string? NoMatric { get; set; }
        public int? CdEmpresa { get; set; }
        public int? CdFilial { get; set; }
        public int NoUser { get; set; }
        public string? Email_Usuario { get; set; }
        public char FlAtivo { get; set; }
        public char? FlNaoRecebeEmail { get; set; }
        public Guid? IdFuncionario { get; set; }

        // =======================
        //  Multi-tenant
        // =======================

        public Guid? TenantPrincipal { get; set; }
        public Guid? TenantId { get; set; }

        // =======================
        //  Autenticação / E-mail
        // =======================

        public string AuthMode { get; set; } = "OnPrem";
        public bool EmailConfirmed { get; set; }
        public string? EmailConfirmationToken { get; set; }
        public DateTime? EmailConfirmationTokenExpiry { get; set; }

        // =======================
        //  Reset de Senha
        // =======================

        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public DateTime? PasswordResetRequestedAt { get; set; }
        public string? PasswordResetRequestedBy { get; set; }
        public int LoginAttempts { get; set; }
        public DateTime? LockedUntil { get; set; }
        public DateTime? LastFailedLoginAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? LastPasswordChangedAt { get; set; }

        // =======================
        //  2FA
        // =======================

        public bool TwoFactorEnabled { get; set; }
        public string? TwoFactorSecret { get; set; }
        public string? TwoFactorBackupCodes { get; set; }

        // =======================
        //  Auditoria
        // =======================

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? LastUserAgent { get; set; }
        public string? LastIpAddress { get; set; }

        // =======================
        //  Relacionamentos
        // =======================

        public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    }
}
