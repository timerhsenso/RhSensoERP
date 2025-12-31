// =============================================================================
// RHSENSOERP - ENTITY CAPANEXOSOCORRENCIA
// =============================================================================
// Módulo: Gestão de Terceiros e Prestadores (CAP)
// Tabela: cap_anexos_ocorrencia
// Schema: dbo
// Multi-tenant: ✅ SIM (TenantId obrigatório)
// Tipo: Append-Only (sem updates/deletes)
// =============================================================================
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;

/// <summary>
/// CapAnexosOcorrencia - Anexos de Ocorrências
/// Tabela: cap_anexos_ocorrencia (Multi-tenant, Append-Only)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "cap_anexos_ocorrencia",
    DisplayName = "CapAnexosOcorrencia",
    CdSistema = "CAP",
    CdFuncao = "CAP_FM_ANEXOSOCORRENCIA",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("cap_anexos_ocorrencia")]
public class CapAnexosOcorrencia
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
    [Column("IdOcorrencia")]
    [Display(Name = "ID Ocorrência")]
    public int IdOcorrencia { get; set; }

    [Required]
    [Column("NomeArquivo", TypeName = "nvarchar(255)")]
    [StringLength(255)]
    [Display(Name = "Nome Arquivo")]
    public string NomeArquivo { get; set; } = string.Empty;

    [Required]
    [Column("CaminhoArquivo", TypeName = "nvarchar(500)")]
    [StringLength(500)]
    [Display(Name = "Caminho Arquivo")]
    public string CaminhoArquivo { get; set; } = string.Empty;

    [Column("TipoArquivo", TypeName = "nvarchar(50)")]
    [StringLength(50)]
    [Display(Name = "Tipo Arquivo")]
    public string? TipoArquivo { get; set; }

    [Column("TamanhoArquivo")]
    [Display(Name = "Tamanho Arquivo (bytes)")]
    public long? TamanhoArquivo { get; set; }

    // Default controlado pelo banco: SYSUTCDATETIME()
    [Required]
    [Column("DataUpload", TypeName = "datetime2(3)")]
    [Display(Name = "Data Upload (UTC)")]
    public DateTime DataUpload { get; set; }

    [Column("UsuarioUpload")]
    [Display(Name = "Usuário Upload")]
    public Guid? UsuarioUpload { get; set; }

    // =========================================================================
    // NAVIGATION PROPERTIES
    // =========================================================================
    [ForeignKey(nameof(IdOcorrencia))]
    public virtual CapOcorrencias? Ocorrencia { get; set; }

    // Comentado: Aguardando implementação completa da entidade Usuario
    // [ForeignKey(nameof(UsuarioUpload))]
    // public virtual Usuario? Usuario { get; set; }
}