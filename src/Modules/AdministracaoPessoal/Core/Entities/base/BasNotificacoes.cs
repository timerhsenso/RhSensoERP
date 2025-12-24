using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// BasNotificacoes - Notificações Geradas do Sistema
/// Tabela: bas_notificacoes (Multi-tenant, Append-Only)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "bas_notificacoes",
    DisplayName = "BasNotificacoes",
    CdSistema = "BAS",
    CdFuncao = "BAS_FM_NOTIFICACOES",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("bas_notificacoes")]
public class BasNotificacoes
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "ID")]
    public long Id { get; set; }

    [Required]
    [Column("TenantId")]
    [Display(Name = "Tenant ID")]
    public Guid TenantId { get; set; }

    [Required]
    [Column("IdTipoNotificacao")]
    [Display(Name = "ID Tipo Notificação")]
    public int IdTipoNotificacao { get; set; }

    [Column("IdUsuarioDestino")]
    [Display(Name = "ID Usuário Destino")]
    public Guid? IdUsuarioDestino { get; set; }

    [Column("EmailDestino", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "E-mail Destino")]
    public string? EmailDestino { get; set; }

    [Required]
    [Column("Titulo", TypeName = "nvarchar(255)")]
    [StringLength(255)]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [Column("Mensagem", TypeName = "nvarchar(max)")]
    [Display(Name = "Mensagem")]
    public string Mensagem { get; set; } = string.Empty;

    [Column("TabelaReferencia", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Tabela Referência")]
    public string? TabelaReferencia { get; set; }

    [Column("IdReferencia")]
    [Display(Name = "ID Referência")]
    public int? IdReferencia { get; set; }

    [Required]
    [Column("Status", TypeName = "nvarchar(50)")]
    [StringLength(50)]
    [Display(Name = "Status")]
    public string Status { get; set; } = "PENDENTE";

    [Column("DataLeitura", TypeName = "datetime2(3)")]
    [Display(Name = "Data Leitura")]
    public DateTime? DataLeitura { get; set; }

    [Column("DataEnvioEmail", TypeName = "datetime2(3)")]
    [Display(Name = "Data Envio E-mail")]
    public DateTime? DataEnvioEmail { get; set; }

    [Column("DataAgendada", TypeName = "datetime2(3)")]
    [Display(Name = "Data Agendada")]
    public DateTime? DataAgendada { get; set; }

    // Default controlado pelo banco: SYSUTCDATETIME()
    [Required]
    [Column("CreatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Data Criação (UTC)")]
    public DateTime CreatedAtUtc { get; set; }

    // Navigation Properties
 //   [ForeignKey(nameof(IdTipoNotificacao))]
 //   public virtual BasTiposNotificacao? TipoNotificacao { get; set; }

 //   [ForeignKey(nameof(IdUsuarioDestino))]
 //   public virtual Usuario? UsuarioDestino { get; set; }
}
