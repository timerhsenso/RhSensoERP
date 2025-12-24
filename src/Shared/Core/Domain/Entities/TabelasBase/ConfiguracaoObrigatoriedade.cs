using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Shared.Core.Domain.Entities.TabelasBase;

/// <summary>
/// Regras de validação configuráveis por empresa.
/// Cada empresa define o que é obrigatório para ela (multi-tenant flexível).
/// Tabela: BASE_ConfiguracaoObrigatoriedade
/// </summary>
[GenerateCrud(
    TableName = "BASE_ConfiguracaoObrigatoriedade",
    DisplayName = "Configuração de Obrigatoriedade",
    CdSistema = "SGT",
    CdFuncao = "SGT_BASE_CONFIGOBRIG",
    GenerateApiController = true
)]
[Table("BASE_ConfiguracaoObrigatoriedade")]
public class ConfiguracaoObrigatoriedade
{
    [Key]
    [Column("Id")]
    [Display(Name = "ID")]
    public int Id { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Multi-Tenant
    // ═══════════════════════════════════════════════════════════════════

    [Column("IdSaas")]
    [Display(Name = "ID SaaS")]
    public Guid? IdSaas { get; set; }

    [Column("CdEmpresa")]
    [Required]
    [Display(Name = "Empresa")]
    public Guid CdEmpresa { get; set; }

    [Column("CdFilial")]
    [Required]
    [Display(Name = "Filial")]
    public Guid CdFilial { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Regra
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Contexto da validação.
    /// Valores: ACESSO_TERCEIRO, ACESSO_VISITANTE, ACESSO_MOTORISTA,
    /// CARGA_RECEBIMENTO, CARGA_PERIGOSA, VEICULO, EPI
    /// </summary>
    [Column("Contexto")]
    [StringLength(30)]
    [Required]
    [Display(Name = "Contexto")]
    public string Contexto { get; set; } = string.Empty;

    [Column("CodigoValidacao")]
    [StringLength(50)]
    [Required]
    [Display(Name = "Código Validação")]
    public string CodigoValidacao { get; set; } = string.Empty;

    [Column("Descricao")]
    [StringLength(150)]
    [Required]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = string.Empty;

    // ═══════════════════════════════════════════════════════════════════
    // Configuração
    // ═══════════════════════════════════════════════════════════════════

    [Column("EhObrigatorio")]
    [Display(Name = "É Obrigatório")]
    public bool EhObrigatorio { get; set; }

    [Column("BloqueiaSeNaoAtender")]
    [Display(Name = "Bloqueia Se Não Atender")]
    public bool BloqueiaSeNaoAtender { get; set; }

    [Column("ApenasAlerta")]
    [Display(Name = "Apenas Alerta")]
    public bool ApenasAlerta { get; set; }

    [Column("ValidadeMinimaEmDias")]
    [Display(Name = "Validade Mínima (Dias)")]
    public int? ValidadeMinimaEmDias { get; set; }

    [Column("MensagemBloqueio")]
    [StringLength(255)]
    [Display(Name = "Mensagem de Bloqueio")]
    public string? MensagemBloqueio { get; set; }

    [Column("MensagemAlerta")]
    [StringLength(255)]
    [Display(Name = "Mensagem de Alerta")]
    public string? MensagemAlerta { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Controle
    // ═══════════════════════════════════════════════════════════════════

    [Column("Ordem")]
    [Display(Name = "Ordem")]
    public int Ordem { get; set; }

    [Column("Ativo")]
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    // ═══════════════════════════════════════════════════════════════════
    // Auditoria
    // ═══════════════════════════════════════════════════════════════════

    [Column("Aud_CreatedAt")]
    [Display(Name = "Criado Em")]
    public DateTime Aud_CreatedAt { get; set; }

    [Column("Aud_UpdatedAt")]
    [Display(Name = "Atualizado Em")]
    public DateTime? Aud_UpdatedAt { get; set; }

    [Column("Aud_IdUsuarioCadastro")]
    [Display(Name = "Usuário Cadastro")]
    public Guid? Aud_IdUsuarioCadastro { get; set; }

    [Column("Aud_IdUsuarioAtualizacao")]
    [Display(Name = "Usuário Atualização")]
    public Guid? Aud_IdUsuarioAtualizacao { get; set; }
}
