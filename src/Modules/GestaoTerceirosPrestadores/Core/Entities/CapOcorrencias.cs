using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;

/// <summary>
/// CapOcorrencias - Ocorrências do Sistema
/// Tabela: cap_ocorrencias (Multi-tenant)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "cap_ocorrencias",
    DisplayName = "CapOcorrencias",
    CdSistema = "CAP",
    CdFuncao = "CAP_FM_OCORRENCIAS",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("cap_ocorrencias")]
public class CapOcorrencias
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "ID")]
    public int Id { get; set; }

    [Required]
    [Column("TenantId")]
    [Display(Name = "Tenant ID")]
    public Guid TenantId { get; set; }

    [Required]
    [Column("IdTipoOcorrencia")]
    [Display(Name = "ID Tipo Ocorrência")]
    public int IdTipoOcorrencia { get; set; }

    [Column("IdAcessoPortaria")]
    [Display(Name = "ID Acesso Portaria")]
    public int? IdAcessoPortaria { get; set; }

    [Column("IdFuncionarioResponsavel")]
    [Display(Name = "ID Funcionário Responsável")]
    public int? IdFuncionarioResponsavel { get; set; }

    [Column("IdColaboradorResponsavel")]
    [Display(Name = "ID Colaborador Responsável")]
    public int? IdColaboradorResponsavel { get; set; }

    [Required]
    [Column("Descricao", TypeName = "nvarchar(500)")]
    [StringLength(500)]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    [Column("DataOcorrencia", TypeName = "datetime2(3)")]
    [Display(Name = "Data Ocorrência")]
    public DateTime DataOcorrencia { get; set; }

    [Column("Local", TypeName = "nvarchar(500)")]
    [StringLength(500)]
    [Display(Name = "Local")]
    public string? Local { get; set; }

    [Column("Observacoes", TypeName = "nvarchar(500)")]
    [StringLength(500)]
    [Display(Name = "Observações")]
    public string? Observacoes { get; set; }

    [Column("Status", TypeName = "nvarchar(50)")]
    [StringLength(50)]
    [Display(Name = "Status")]
    public string? Status { get; set; }

    [Required]
    [Column("Ativo")]
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    // Auditoria (defaults controlados pelo banco: SYSUTCDATETIME())
    [Required]
    [Column("CreatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Data Criação (UTC)")]
    public DateTime CreatedAtUtc { get; set; }

    [Column("CreatedByUserId")]
    [Display(Name = "Criado Por")]
    public Guid? CreatedByUserId { get; set; }

    [Required]
    [Column("UpdatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Data Atualização (UTC)")]
    public DateTime UpdatedAtUtc { get; set; }

    [Column("UpdatedByUserId")]
    [Display(Name = "Atualizado Por")]
    public Guid? UpdatedByUserId { get; set; }

    // Navigation Properties
    [ForeignKey(nameof(IdTipoOcorrencia))]
    public virtual CapTiposOcorrencia? TipoOcorrencia { get; set; }

    [ForeignKey(nameof(IdColaboradorResponsavel))]
    public virtual CapColaboradoresFornecedor? ColaboradorResponsavel { get; set; }

    // NOTA: Navigation para gtc_acessos_portaria comentada - módulo GTC não existe ainda
    // [ForeignKey(nameof(IdAcessoPortaria))]
    // public virtual GtcAcessosPortaria? AcessoPortaria { get; set; }

   // [ForeignKey(nameof(CreatedByUserId))]
   // public virtual Usuario? CreatedByUser { get; set; }

  //  [ForeignKey(nameof(UpdatedByUserId))]
  //  public virtual Usuario? UpdatedByUser { get; set; }

    // Inverse Navigation
    [InverseProperty(nameof(CapAnexosOcorrencia.Ocorrencia))]
    public virtual ICollection<CapAnexosOcorrencia> Anexos { get; set; } = new List<CapAnexosOcorrencia>();
}