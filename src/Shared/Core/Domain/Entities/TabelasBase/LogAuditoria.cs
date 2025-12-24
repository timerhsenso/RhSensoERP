using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Shared.Core.Domain.Entities.TabelasBase;

/// <summary>
/// Log centralizado de todas as alterações críticas do sistema.
/// Atende requisitos de compliance e rastreabilidade.
/// Tabela: BASE_LogAuditoria
/// </summary>
[Table("BASE_LogAuditoria")]
public class LogAuditoria
{
    [Key]
    [Column("Id")]
    [Display(Name = "ID")]
    public long Id { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Multi-Tenant
    // ═══════════════════════════════════════════════════════════════════

    [Column("IdSaas")]
    [Display(Name = "ID SaaS")]
    public Guid? IdSaas { get; set; }

    [Column("CdEmpresa")]
    [Display(Name = "Empresa")]
    public Guid? CdEmpresa { get; set; }

    [Column("CdFilial")]
    [Display(Name = "Filial")]
    public Guid? CdFilial { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Dados da Operação
    // ═══════════════════════════════════════════════════════════════════

    [Column("DataHora")]
    [Required]
    [Display(Name = "Data/Hora")]
    public DateTime DataHora { get; set; }

    [Column("TabelaAfetada")]
    [StringLength(100)]
    [Required]
    [Display(Name = "Tabela Afetada")]
    public string TabelaAfetada { get; set; } = string.Empty;

    [Column("IdRegistroAfetado")]
    [StringLength(50)]
    [Required]
    [Display(Name = "ID Registro Afetado")]
    public string IdRegistroAfetado { get; set; } = string.Empty;

    /// <summary>
    /// Tipo da operação: I=Insert, U=Update, D=Delete
    /// </summary>
    [Column("TipoOperacao")]
    [StringLength(1)]
    [Required]
    [Display(Name = "Tipo Operação")]
    public string TipoOperacao { get; set; } = string.Empty;

    // ═══════════════════════════════════════════════════════════════════
    // Usuário
    // ═══════════════════════════════════════════════════════════════════

    [Column("IdUsuario")]
    [Display(Name = "ID Usuário")]
    public Guid? IdUsuario { get; set; }

    [Column("NomeUsuario")]
    [StringLength(100)]
    [Display(Name = "Nome Usuário")]
    public string? NomeUsuario { get; set; }

    [Column("IpOrigem")]
    [StringLength(45)]
    [Display(Name = "IP Origem")]
    public string? IpOrigem { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Dados Alterados (JSON)
    // ═══════════════════════════════════════════════════════════════════

    [Column("DadosAnteriores")]
    [Display(Name = "Dados Anteriores (JSON)")]
    public string? DadosAnteriores { get; set; }

    [Column("DadosNovos")]
    [Display(Name = "Dados Novos (JSON)")]
    public string? DadosNovos { get; set; }

    [Column("CamposAlterados")]
    [StringLength(500)]
    [Display(Name = "Campos Alterados")]
    public string? CamposAlterados { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Contexto
    // ═══════════════════════════════════════════════════════════════════

    [Column("Modulo")]
    [StringLength(50)]
    [Display(Name = "Módulo")]
    public string? Modulo { get; set; }

    [Column("Funcionalidade")]
    [StringLength(100)]
    [Display(Name = "Funcionalidade")]
    public string? Funcionalidade { get; set; }

    [Column("Observacao")]
    [StringLength(500)]
    [Display(Name = "Observação")]
    public string? Observacao { get; set; }
}
