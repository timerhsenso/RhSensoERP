using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Shared.Core.Domain.Entities.TabelasBase;

/// <summary>
/// Graus de parentesco para contatos de emergência.
/// Tabela: BASE_TipoParentesco
/// </summary>
[GenerateCrud(
    TableName = "BASE_TipoParentesco",
    DisplayName = "Tipo de Parentesco",
    CdSistema = "SGT",
    CdFuncao = "SGT_BASE_TIPOPARENTESCO",
    GenerateApiController = true
)]
[Table("BASE_TipoParentesco")]
public class TipoParentesco
{
    [Key]
    [Column("Id")]
    [Display(Name = "ID")]
    public byte Id { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Multi-Tenant (NULL = registro padrão do sistema)
    // ═══════════════════════════════════════════════════════════════════

    [Column("IdSaas")]
    [Display(Name = "ID SaaS")]
    public Guid? IdSaas { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Dados
    // ═══════════════════════════════════════════════════════════════════

    [Column("Codigo")]
    [StringLength(20)]
    [Required]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Column("Descricao")]
    [StringLength(50)]
    [Required]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = string.Empty;

    [Column("Ordem")]
    [Display(Name = "Ordem")]
    public byte Ordem { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Controle
    // ═══════════════════════════════════════════════════════════════════

    [Column("Ativo")]
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;
}
