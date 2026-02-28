// =================================================================================================
// RhSensoERP • Segurança • Entity
// =================================================================================================
// Entidade:        UsuarioGrupo
// Tabela:          dbo.usrh1
// Natureza:        Modernizada (Guid PK + TenantId)
// Chave primária:  Id (uniqueidentifier, newsequentialid)
// Finalidade:      Vínculo de usuários a grupos com período de validade
// Compatibilidade: SQL Server 2019+
// =================================================================================================
// FK: idgrupodeusuario → dbo.gurh1(id)  (GrupoDeUsuario)
// FK: idusuario        → dbo.tuse1(id)  (Usuario - módulo Identity)
// UK: (cdusuario, cdsistema, cdgruser, dtinival)
// =================================================================================================

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.Seguranca.Core.Entities;

/// <summary>
/// Vínculo de usuários a grupos de acesso com período de validade (tabela usrh1).
/// PK simples: Id (Guid, newsequentialid).
/// </summary>
[GenerateCrud(
    TableName = "usrh1",
    DisplayName = "Vínculo Usuário-Grupo",
    CdSistema = "SEG",
    CdFuncao = "SEG_FM_USUARIOGRUPO",
    IsLegacyTable = false,
    GenerateApiController = true
 //   ApiRoute = "seguranca/usuariogrupo"
)]
[Table("usrh1", Schema = "dbo")]
public class UsuarioGrupo
{
    // =============================================================================================
    // CHAVE PRIMÁRIA
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.usrh1.id (uniqueidentifier, default newsequentialid()) - PK.
    /// </summary>
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    // =============================================================================================
    // MULTI-TENANT
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.usrh1.TenantId (uniqueidentifier, nullable).
    /// </summary>
    [Column("TenantId")]
    public Guid? TenantId { get; set; }

    // =============================================================================================
    // DADOS LEGADOS (varchar/char)
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.usrh1.cdusuario (varchar(30)) - código do usuário legado.
    /// </summary>
    [Required, StringLength(30)]
    [Column("cdusuario", TypeName = "varchar(30)")]
    public string CdUsuario { get; set; } = string.Empty;

    /// <summary>
    /// SQL: dbo.usrh1.cdgruser (varchar(30)) - código do grupo legado.
    /// </summary>
    [Required, StringLength(30)]
    [Column("cdgruser", TypeName = "varchar(30)")]
    public string CdGrUser { get; set; } = string.Empty;

    /// <summary>
    /// SQL: dbo.usrh1.cdsistema (char(10)) - código do sistema.
    /// </summary>
    [StringLength(10)]
    [Column("cdsistema", TypeName = "char(10)")]
    public string? CdSistema { get; set; }

    // =============================================================================================
    // PERÍODO DE VALIDADE
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.usrh1.dtinival (datetime) - início da validade.
    /// </summary>
    [Required]
    [Column("dtinival")]
    public DateTime DtIniVal { get; set; }

    /// <summary>
    /// SQL: dbo.usrh1.dtfimval (datetime, nullable) - fim da validade.
    /// </summary>
    [Column("dtfimval")]
    public DateTime? DtFimVal { get; set; }

    // =============================================================================================
    // FK's (Guid) - MODERNIZADAS
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.usrh1.idusuario (uniqueidentifier) - FK → dbo.tuse1(id).
    /// </summary>
    [Column("idusuario")]
    public Guid? IdUsuario { get; set; }

    /// <summary>
    /// SQL: dbo.usrh1.idgrupodeusuario (uniqueidentifier) - FK → dbo.gurh1(id).
    /// </summary>
    [Column("idgrupodeusuario")]
    public Guid? IdGrupoDeUsuario { get; set; }

    // =============================================================================================
    // RELACIONAMENTOS
    // =============================================================================================

    /// <summary>
    /// Grupo de usuário vinculado (FK → gurh1).
    /// </summary>
  //  [ForeignKey(nameof(IdGrupoDeUsuario))]
  //  public virtual GrupoDeUsuario? GrupoDeUsuario { get; set; }

    // ⚠️ Navegação para Usuario omitida - pertence ao módulo Identity (tuse1).
    // Para evitar dependência circular entre Seguranca ↔ Identity.
    // Usar IdUsuario (Guid) para joins manuais quando necessário.
}