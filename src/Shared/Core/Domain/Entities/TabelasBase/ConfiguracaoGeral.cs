using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Shared.Core.Domain.Entities.TabelasBase;

/// <summary>
/// Configurações gerais do sistema por empresa/filial.
/// Define comportamentos padrão e parâmetros globais.
/// Tabela: BASE_ConfiguracaoGeral
/// </summary>
[GenerateCrud(
    TableName = "BASE_ConfiguracaoGeral",
    DisplayName = "Configuração Geral",
    CdSistema = "SGT",
    CdFuncao = "SGT_BASE_CONFIGGERAL",
    GenerateApiController = true
)]
[Table("BASE_ConfiguracaoGeral")]
public class ConfiguracaoGeral
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
    // Configurações de Agendamento
    // ═══════════════════════════════════════════════════════════════════

    [Column("ExigeAgendamentoCarga")]
    [Display(Name = "Exige Agendamento de Carga")]
    public bool ExigeAgendamentoCarga { get; set; }

    [Column("DiasAntecedenciaMinima")]
    [Display(Name = "Dias Antecedência Mínima")]
    public int? DiasAntecedenciaMinima { get; set; }

    [Column("PermiteAgendamentoMesmoDia")]
    [Display(Name = "Permite Agendamento Mesmo Dia")]
    public bool PermiteAgendamentoMesmoDia { get; set; } = true;

    [Column("ExigeAprovacaoAgendamento")]
    [Display(Name = "Exige Aprovação de Agendamento")]
    public bool ExigeAprovacaoAgendamento { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Configurações de Acesso
    // ═══════════════════════════════════════════════════════════════════

    [Column("ExigeFotoEntrada")]
    [Display(Name = "Exige Foto na Entrada")]
    public bool ExigeFotoEntrada { get; set; }

    [Column("ExigeBiometriaEntrada")]
    [Display(Name = "Exige Biometria na Entrada")]
    public bool ExigeBiometriaEntrada { get; set; }

    [Column("ExigeAssinaturaDigital")]
    [Display(Name = "Exige Assinatura Digital")]
    public bool ExigeAssinaturaDigital { get; set; }

    [Column("TempoMaximoPermanenciaHoras")]
    [Display(Name = "Tempo Máximo Permanência (Horas)")]
    public int? TempoMaximoPermanenciaHoras { get; set; }

    [Column("BloqueiaDocumentoVencido")]
    [Display(Name = "Bloqueia Documento Vencido")]
    public bool BloqueiaDocumentoVencido { get; set; } = true;

    [Column("DiasAlertaVencimento")]
    [Display(Name = "Dias Alerta Vencimento")]
    public int DiasAlertaVencimento { get; set; } = 30;

    [Column("DiasAlertaVencimentoCritico")]
    [Display(Name = "Dias Alerta Vencimento Crítico")]
    public int DiasAlertaVencimentoCritico { get; set; } = 7;

    // ═══════════════════════════════════════════════════════════════════
    // Configurações de Carga
    // ═══════════════════════════════════════════════════════════════════

    [Column("PermiteRecusaParcial")]
    [Display(Name = "Permite Recusa Parcial")]
    public bool PermiteRecusaParcial { get; set; } = true;

    [Column("ExigePesagemEntrada")]
    [Display(Name = "Exige Pesagem na Entrada")]
    public bool ExigePesagemEntrada { get; set; }

    [Column("ExigePesagemSaida")]
    [Display(Name = "Exige Pesagem na Saída")]
    public bool ExigePesagemSaida { get; set; }

    [Column("ToleranciaKgPesagem")]
    [Display(Name = "Tolerância Pesagem (Kg)")]
    public decimal? ToleranciaKgPesagem { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Configurações de Crachá
    // ═══════════════════════════════════════════════════════════════════

    [Column("UsaCrachaProvisorio")]
    [Display(Name = "Usa Crachá Provisório")]
    public bool UsaCrachaProvisorio { get; set; } = true;

    [Column("ExigeDevolucaoCracha")]
    [Display(Name = "Exige Devolução de Crachá")]
    public bool ExigeDevolucaoCracha { get; set; } = true;

    // ═══════════════════════════════════════════════════════════════════
    // Horários de Operação
    // ═══════════════════════════════════════════════════════════════════

    [Column("HorarioInicioCarga")]
    [Display(Name = "Horário Início Carga")]
    public TimeSpan? HorarioInicioCarga { get; set; }

    [Column("HorarioFimCarga")]
    [Display(Name = "Horário Fim Carga")]
    public TimeSpan? HorarioFimCarga { get; set; }

    [Column("HorarioInicioVisitante")]
    [Display(Name = "Horário Início Visitante")]
    public TimeSpan? HorarioInicioVisitante { get; set; }

    [Column("HorarioFimVisitante")]
    [Display(Name = "Horário Fim Visitante")]
    public TimeSpan? HorarioFimVisitante { get; set; }

    [Column("PermiteCargaFimDeSemana")]
    [Display(Name = "Permite Carga Fim de Semana")]
    public bool PermiteCargaFimDeSemana { get; set; }

    [Column("PermiteVisitanteFimDeSemana")]
    [Display(Name = "Permite Visitante Fim de Semana")]
    public bool PermiteVisitanteFimDeSemana { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Integrações
    // ═══════════════════════════════════════════════════════════════════

    [Column("IntegracaoCatracaAtiva")]
    [Display(Name = "Integração Catraca Ativa")]
    public bool IntegracaoCatracaAtiva { get; set; }

    [Column("IntegracaoBalancaAtiva")]
    [Display(Name = "Integração Balança Ativa")]
    public bool IntegracaoBalancaAtiva { get; set; }

    [Column("IntegracaoReconhecimentoFacial")]
    [Display(Name = "Integração Reconhecimento Facial")]
    public bool IntegracaoReconhecimentoFacial { get; set; }

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
