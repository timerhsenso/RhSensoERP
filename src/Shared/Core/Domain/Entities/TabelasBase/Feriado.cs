using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Shared.Core.Domain.Entities.TabelasBase;

/// <summary>
/// Feriados nacionais, estaduais e municipais.
/// Usado para validação de agendamentos e horários de operação.
/// Tabela: BASE_Feriado
/// </summary>
[GenerateCrud(
    TableName = "BASE_Feriado",
    DisplayName = "Feriado",
    CdSistema = "SGT",
    CdFuncao = "SGT_BASE_FERIADO",
    GenerateApiController = true
)]
[Table("BASE_Feriado")]
public class Feriado
{
    [Key]
    [Column("Id")]
    [Display(Name = "ID")]
    public int Id { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Multi-Tenant (NULL = feriado nacional)
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
    // Localização (para feriados estaduais/municipais)
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// UF do feriado estadual. FK para BASE_UF.Sigla
    /// </summary>
    [Column("UF")]
    [StringLength(2)]
    [Display(Name = "UF")]
    public string? UF { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Dados
    // ═══════════════════════════════════════════════════════════════════

    [Column("Data")]
    [Required]
    [Display(Name = "Data")]
    [DataType(DataType.Date)]
    public DateTime Data { get; set; }

    [Column("Descricao")]
    [StringLength(100)]
    [Required]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Tipo do feriado.
    /// Valores: NACIONAL, ESTADUAL, MUNICIPAL, PONTO_FACULTATIVO
    /// </summary>
    [Column("Tipo")]
    [StringLength(20)]
    [Required]
    [Display(Name = "Tipo")]
    public string Tipo { get; set; } = string.Empty;

    [Column("Recorrente")]
    [Display(Name = "Recorrente (Anual)")]
    public bool Recorrente { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Impacto nas Operações
    // ═══════════════════════════════════════════════════════════════════

    [Column("BloqueiaAcessoTerceiros")]
    [Display(Name = "Bloqueia Acesso Terceiros")]
    public bool BloqueiaAcessoTerceiros { get; set; }

    [Column("BloqueiaAcessoVisitantes")]
    [Display(Name = "Bloqueia Acesso Visitantes")]
    public bool BloqueiaAcessoVisitantes { get; set; }

    [Column("BloqueiaRecebimentoCarga")]
    [Display(Name = "Bloqueia Recebimento de Carga")]
    public bool BloqueiaRecebimentoCarga { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Controle
    // ═══════════════════════════════════════════════════════════════════

    [Column("Ativo")]
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    // ═══════════════════════════════════════════════════════════════════
    // Auditoria
    // ═══════════════════════════════════════════════════════════════════

    [Column("Aud_CreatedAt")]
    [Display(Name = "Criado Em")]
    public DateTime Aud_CreatedAt { get; set; }

    [Column("Aud_IdUsuarioCadastro")]
    [Display(Name = "Usuário Cadastro")]
    public Guid? Aud_IdUsuarioCadastro { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Navigation Properties
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// UF do feriado (se estadual)
    /// </summary>
    [ForeignKey("UF")]
    public virtual UF? UFNavigation { get; set; }
}
