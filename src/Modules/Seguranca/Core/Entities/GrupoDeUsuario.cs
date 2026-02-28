// =================================================================================================
// RhSensoERP • Segurança • Entity
// =================================================================================================
// Entidade:        GrupoDeUsuario
// Tabela:          dbo.gurh1
// Natureza:        Legada
// Chave primária:  (CdSistema, CdGrUser)
// Finalidade:      Grupos de usuários para controle de acesso
// Compatibilidade: SQL Server 2019+
// =================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.Seguranca.Core.Entities;

/// <summary>
/// Entidade que representa grupos de usuários do sistema (tabela gurh1).
/// PK composta: (CdSistema, CdGrUser)
/// </summary>
[GenerateCrud(
    TableName = "gurh1",
    DisplayName = "Grupos de Usuário",
    CdSistema = "SEG",
    CdFuncao = "SEG_FM_GRUPOUSUARIO",
    IsLegacyTable = true,
    GenerateApiController = true,
    SupportsBatchDelete = false
)]
[Table("gurh1")]
[PrimaryKey(nameof(CdSistema), nameof(CdGrUser))]
public class GrupoDeUsuario
{
    // =============================================================================================
    // CHAVE PRIMÁRIA (PK composta)
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.gurh1.cdsistema (char(10)) - parte da PK composta.
    /// </summary>
    [Key]
    [Required, StringLength(10)]
    [Column("cdsistema", TypeName = "char(10)")]
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// SQL: dbo.gurh1.cdgruser (varchar(30)) - parte da PK composta.
    /// </summary>
    [Key]
    [Required, StringLength(30)]
    [Column("cdgruser", TypeName = "varchar(30)")]
    public string CdGrUser { get; set; } = string.Empty;

    // =============================================================================================
    // IDENTIFICADOR ÚNICO (GUID)
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.gurh1.id (uniqueidentifier) - identificador único com default newsequentialid().
    /// Usado como chave de lookup alternativa nas APIs.
    /// </summary>
    [Required]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    // =============================================================================================
    // MULTI-TENANT
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.gurh1.TenantId (uniqueidentifier NULL) - identificador do tenant para isolamento SaaS.
    /// </summary>
    [Column("TenantId")]
    public Guid? TenantId { get; set; }

    // =============================================================================================
    // DADOS
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.gurh1.dcgruser (varchar(60)) - descrição do grupo.
    /// </summary>
    [StringLength(60)]
    [Column("dcgruser", TypeName = "varchar(60)")]
    public string? DcGrUser { get; set; }

    // =============================================================================================
    // RELACIONAMENTOS
    // =============================================================================================

    /// <summary>
    /// Sistema ao qual este grupo pertence.
    /// </summary>
    public virtual Tsistema Sistema { get; set; } = null!;

    /// <summary>
    /// Vínculos de usuários neste grupo (usrh1).
    /// </summary>
    public virtual ICollection<UsuarioGrupo> UsuarioGrupos { get; set; } = new List<UsuarioGrupo>();

    /// <summary>
    /// Habilitações de funções deste grupo (hbrh1).
    /// </summary>
    public virtual ICollection<HabilitacaoGrupo> Habilitacoes { get; set; } = new List<HabilitacaoGrupo>();
}