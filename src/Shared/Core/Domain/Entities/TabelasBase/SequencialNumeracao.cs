using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Shared.Core.Domain.Entities.TabelasBase;

/// <summary>
/// Controle de numeração sequencial para protocolos, autorizações, etc.
/// Garante unicidade mesmo em ambiente concorrente.
/// Tabela: BASE_SequencialNumeracao
/// </summary>
[Table("BASE_SequencialNumeracao")]
public class SequencialNumeracao
{
    [Key]
    [Column("Id")]
    [Display(Name = "ID")]
    public int Id { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Multi-Tenant
    // ═══════════════════════════════════════════════════════════════════

    [Column("CdEmpresa")]
    [Required]
    [Display(Name = "Empresa")]
    public Guid CdEmpresa { get; set; }

    [Column("CdFilial")]
    [Required]
    [Display(Name = "Filial")]
    public Guid CdFilial { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Tipo de Numeração
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Tipo de numeração sequencial.
    /// Valores: PROTOCOLO_ACESSO, AUTORIZACAO, AGENDAMENTO, OCORRENCIA,
    /// ORDEM_SERVICO, NUMERO_CRACHA, TICKET_PESAGEM
    /// </summary>
    [Column("TipoNumeracao")]
    [StringLength(30)]
    [Required]
    [Display(Name = "Tipo Numeração")]
    public string TipoNumeracao { get; set; } = string.Empty;

    [Column("Prefixo")]
    [StringLength(10)]
    [Display(Name = "Prefixo")]
    public string? Prefixo { get; set; }

    [Column("Ano")]
    [Required]
    [Display(Name = "Ano")]
    public int Ano { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Sequencial
    // ═══════════════════════════════════════════════════════════════════

    [Column("UltimoNumero")]
    [Display(Name = "Último Número")]
    public long UltimoNumero { get; set; }
}
