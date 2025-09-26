// Src/Core/MSO/Entities/MsoAtendimento.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_atendimento")]
public class MsoAtendimento
{
    [Key, Column("cod_empregado", Order = 1)]
    public int CodEmpregado { get; set; }

    [Key, Column("data_atendimento", Order = 2)]
    public DateTime DataAtendimento { get; set; }

    [Key, Column("cod_registro", Order = 3)]
    public int CodRegistro { get; set; }

    [Column("cod_cid")]
    [StringLength(10)]
    [Required]
    public string CodCid { get; set; } = string.Empty;

    [Column("desc_ocorrencia")]
    [StringLength(4000)]
    public string? DescOcorrencia { get; set; }

    [Column("cod_profsaude")]
    public int? CodProfSaude { get; set; }

    [Column("tipo_atendimento")]
    [StringLength(30)]
    public string? TipoAtendimento { get; set; } // CONSULTA, EMERGENCIA, RETORNO

    [Column("motivo_atendimento")]
    [StringLength(100)]
    public string? MotivoAtendimento { get; set; }

    [Column("hora_inicio")]
    [StringLength(5)]
    public string? HoraInicio { get; set; }

    [Column("hora_fim")]
    [StringLength(5)]
    public string? HoraFim { get; set; }

    [Column("pressao_arterial")]
    [StringLength(20)]
    public string? PressaoArterial { get; set; }

    [Column("temperatura")]
    public decimal? Temperatura { get; set; }

    [Column("peso")]
    public decimal? Peso { get; set; }

    [Column("altura")]
    public decimal? Altura { get; set; }

    [Column("frequencia_cardiaca")]
    public int? FrequenciaCardiaca { get; set; }

    [Column("saturacao_oxigenio")]
    public decimal? SaturacaoOxigenio { get; set; }

    [Column("queixa_principal")]
    [StringLength(1000)]
    public string? QueixaPrincipal { get; set; }

    [Column("historia_doenca_atual")]
    [StringLength(4000)]
    public string? HistoriaDoencaAtual { get; set; }

    [Column("exame_fisico")]
    [StringLength(4000)]
    public string? ExameFisico { get; set; }

    [Column("hipotese_diagnostica")]
    [StringLength(1000)]
    public string? HipoteseDiagnostica { get; set; }

    [Column("conduta")]
    [StringLength(2000)]
    public string? Conduta { get; set; }

    [Column("prescricao")]
    [StringLength(4000)]
    public string? Prescricao { get; set; }

    [Column("observacoes")]
    [StringLength(2000)]
    public string? Observacoes { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = "EM_ANDAMENTO"; // EM_ANDAMENTO, CONCLUIDO, CANCELADO

    [Column("prioridade")]
    [StringLength(10)]
    public string? Prioridade { get; set; } = "NORMAL"; // BAIXA, NORMAL, ALTA, URGENTE

    [Column("origem_atendimento")]
    [StringLength(30)]
    public string? OrigemAtendimento { get; set; } // AGENDA, EMERGENCIA, DEMANDA_ESPONTANEA

    [Column("encaminhamento")]
    [StringLength(1000)]
    public string? Encaminhamento { get; set; }

    [Column("retorno_necessario")]
    public bool RetornoNecessario { get; set; } = false;

    [Column("data_retorno")]
    public DateTime? DataRetorno { get; set; }

    [Column("afastamento_trabalho")]
    public bool AfastamentoTrabalho { get; set; } = false;

    [Column("dias_afastamento")]
    public int? DiasAfastamento { get; set; }

    [Column("apto_trabalho")]
    [StringLength(1)]
    public string? AptoTrabalho { get; set; } // S/N/R (Sim/Não/Restrito)

    [Column("restricoes")]
    [StringLength(1000)]
    public string? Restricoes { get; set; }

    [Column("data_criacao")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    [Column("data_atualizacao")]
    public DateTime? DataAtualizacao { get; set; }

    [Column("usuario_criacao")]
    [StringLength(50)]
    public string? UsuarioCriacao { get; set; }

    [Column("usuario_atualizacao")]
    [StringLength(50)]
    public string? UsuarioAtualizacao { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    [Column("numero_protocolo")]
    [StringLength(20)]
    public string? NumeroProtocolo { get; set; }

    [Column("arquivo_anexo")]
    [StringLength(500)]
    public string? ArquivoAnexo { get; set; }

    // Relacionamentos
    [ForeignKey(nameof(CodEmpregado))]
    public virtual MsoEmpregado? Empregado { get; set; }

    [ForeignKey(nameof(CodCid))]
    public virtual MsoCid? Cid { get; set; }

    [ForeignKey(nameof(CodProfSaude))]
    public virtual MsoProfsaude? ProfSaude { get; set; }

    public virtual ICollection<MsoAtendimentoItens> AtendimentoItens { get; set; } = new List<MsoAtendimentoItens>();
    public virtual ICollection<MsoAtendimentoMedicamento> AtendimentoMedicamentos { get; set; } = new List<MsoAtendimentoMedicamento>();

    // Propriedades calculadas
    [NotMapped]
    public TimeSpan? DuracaoAtendimento
    {
        get
        {
            if (string.IsNullOrEmpty(HoraInicio) || string.IsNullOrEmpty(HoraFim))
                return null;

            if (TimeSpan.TryParse(HoraInicio, out var inicio) &&
                TimeSpan.TryParse(HoraFim, out var fim))
            {
                return fim.Subtract(inicio);
            }
            return null;
        }
    }

    [NotMapped]
    public DateTime DataHoraInicio
    {
        get
        {
            if (TimeSpan.TryParse(HoraInicio, out var hora))
                return DataAtendimento.Date.Add(hora);
            return DataAtendimento;
        }
    }

    [NotMapped]
    public DateTime? DataHoraFim
    {
        get
        {
            if (!string.IsNullOrEmpty(HoraFim) && TimeSpan.TryParse(HoraFim, out var hora))
                return DataAtendimento.Date.Add(hora);
            return null;
        }
    }

    [NotMapped]
    public decimal? IMC
    {
        get
        {
            if (Peso.HasValue && Altura.HasValue && Altura > 0)
            {
                var alturaM = Altura.Value / 100; // Converte cm para metros
                return Math.Round(Peso.Value / (alturaM * alturaM), 2);
            }
            return null;
        }
    }

    [NotMapped]
    public string ClassificacaoIMC
    {
        get
        {
            if (!IMC.HasValue) return "Não calculado";

            return IMC.Value switch
            {
                < 18.5m => "Baixo peso",
                >= 18.5m and < 25m => "Peso normal",
                >= 25m and < 30m => "Sobrepeso",
                >= 30m and < 35m => "Obesidade Grau I",
                >= 35m and < 40m => "Obesidade Grau II",
                >= 40m => "Obesidade Grau III",
                _ => "Indefinido"
            };
        }
    }

    [NotMapped]
    public string StatusDescricao => Status switch
    {
        "EM_ANDAMENTO" => "Em Andamento",
        "CONCLUIDO" => "Concluído",
        "CANCELADO" => "Cancelado",
        "AGUARDANDO_EXAMES" => "Aguardando Exames",
        "RETORNO_AGENDADO" => "Retorno Agendado",
        _ => "Indefinido"
    };

    [NotMapped]
    public string PrioridadeDescricao => Prioridade switch
    {
        "BAIXA" => "Baixa",
        "NORMAL" => "Normal",
        "ALTA" => "Alta",
        "URGENTE" => "Urgente",
        "EMERGENCIA" => "Emergência",
        _ => "Normal"
    };

    [NotMapped]
    public bool PodeCancelar => Status == "EM_ANDAMENTO";

    [NotMapped]
    public bool PodeEditar => Status != "CANCELADO";

    [NotMapped]
    public bool PodeImprimir => Status == "CONCLUIDO";

    [NotMapped]
    public bool TemMedicamentos => AtendimentoMedicamentos?.Any() == true;

    [NotMapped]
    public bool TemRestricoes => !string.IsNullOrWhiteSpace(Restricoes);

    [NotMapped]
    public bool EstaAfastado => AfastamentoTrabalho && DiasAfastamento > 0;

    [NotMapped]
    public DateTime? DataFimAfastamento
    {
        get
        {
            if (AfastamentoTrabalho && DiasAfastamento.HasValue)
                return DataAtendimento.AddDays(DiasAfastamento.Value);
            return null;
        }
    }

    [NotMapped]
    public bool AfastamentoVencido
    {
        get
        {
            var dataFim = DataFimAfastamento;
            return dataFim.HasValue && dataFim.Value.Date < DateTime.Today;
        }
    }

    // Métodos de negócio
    public void ConcluirAtendimento(string usuario)
    {
        if (Status != "EM_ANDAMENTO")
            throw new InvalidOperationException("Só é possível concluir atendimentos em andamento.");

        if (string.IsNullOrWhiteSpace(CodCid))
            throw new InvalidOperationException("CID é obrigatório para concluir o atendimento.");

        Status = "CONCLUIDO";
        HoraFim = DateTime.Now.ToString("HH:mm");
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void CancelarAtendimento(string usuario, string motivo = null)
    {
        if (Status == "CONCLUIDO")
            throw new InvalidOperationException("Não é possível cancelar atendimentos concluídos.");

        Status = "CANCELADO";

        if (!string.IsNullOrEmpty(motivo))
            Observacoes = $"{Observacoes}\nCancelado: {motivo}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AgendarRetorno(DateTime dataRetorno, string usuario, string observacao = null)
    {
        if (Status != "CONCLUIDO")
            throw new InvalidOperationException("Só é possível agendar retorno para atendimentos concluídos.");

        RetornoNecessario = true;
        DataRetorno = dataRetorno;
        Status = "RETORNO_AGENDADO";

        if (!string.IsNullOrEmpty(observacao))
            Observacoes = $"{Observacoes}\nRetorno agendado: {observacao}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void DefinirAfastamento(int diasAfastamento, string usuario, string justificativa = null)
    {
        if (diasAfastamento <= 0)
            throw new ArgumentException("Dias de afastamento deve ser maior que zero.");

        AfastamentoTrabalho = true;
        DiasAfastamento = diasAfastamento;
        AptoTrabalho = "N";

        if (!string.IsNullOrEmpty(justificativa))
            Observacoes = $"{Observacoes}\nAfastamento: {diasAfastamento} dias - {justificativa}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void DefinirRestricoes(string restricoes, string usuario)
    {
        if (string.IsNullOrWhiteSpace(restricoes))
            throw new ArgumentException("Restrições não podem estar vazias.");

        Restricoes = restricoes;
        AptoTrabalho = "R"; // Restrito

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void RemoverRestricoes(string usuario)
    {
        Restricoes = null;
        AptoTrabalho = "S"; // Sim

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void GerarProtocolo()
    {
        if (string.IsNullOrEmpty(NumeroProtocolo))
        {
            var ano = DataAtendimento.Year;
            var mes = DataAtendimento.Month.ToString("D2");
            var dia = DataAtendimento.Day.ToString("D2");
            var sequencial = CodRegistro.ToString("D4");

            NumeroProtocolo = $"AT{ano}{mes}{dia}{sequencial}";
        }
    }

    public void AdicionarMedicamento(int codMedicamento, string dosagem, string posologia, string usuario)
    {
        var medicamento = new MsoAtendimentoMedicamento
        {
            CodAtendimento = CodRegistro,
            CodMedicamento = codMedicamento,
            Dosagem = dosagem,
            Posologia = posologia,
            UsuarioCriacao = usuario
        };

        AtendimentoMedicamentos.Add(medicamento);
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    // Validações
    public bool IsHorarioValido()
    {
        if (string.IsNullOrEmpty(HoraInicio)) return true;

        if (!TimeSpan.TryParse(HoraInicio, out var inicio))
            return false;

        if (!string.IsNullOrEmpty(HoraFim))
        {
            if (!TimeSpan.TryParse(HoraFim, out var fim))
                return false;

            return fim > inicio;
        }

        return true;
    }

    public bool IsCidValido()
    {
        return !string.IsNullOrWhiteSpace(CodCid) && CodCid.Length <= 10;
    }

    public bool TemDadosVitais()
    {
        return !string.IsNullOrEmpty(PressaoArterial) ||
               Temperatura.HasValue ||
               FrequenciaCardiaca.HasValue ||
               SaturacaoOxigenio.HasValue;
    }

    public bool TemDadosAntropometricos()
    {
        return Peso.HasValue || Altura.HasValue;
    }

    public bool TemQueixas()
    {
        return !string.IsNullOrWhiteSpace(QueixaPrincipal) ||
               !string.IsNullOrWhiteSpace(HistoriaDoencaAtual);
    }

    public bool TemExameFisico()
    {
        return !string.IsNullOrWhiteSpace(ExameFisico);
    }

    public bool TemDiagnostico()
    {
        return !string.IsNullOrWhiteSpace(HipoteseDiagnostica) && !string.IsNullOrWhiteSpace(CodCid);
    }

    public bool TemConduta()
    {
        return !string.IsNullOrWhiteSpace(Conduta);
    }

    public bool IsAtendimentoCompleto()
    {
        return TemQueixas() && TemExameFisico() && TemDiagnostico() && TemConduta();
    }

    // Métodos de relatório
    public Dictionary<string, object> ToResumoAtendimento()
    {
        return new Dictionary<string, object>
        {
            ["Protocolo"] = NumeroProtocolo ?? "Não gerado",
            ["Data"] = DataAtendimento.ToString("dd/MM/yyyy"),
            ["Hora_Inicio"] = HoraInicio ?? "Não informado",
            ["Hora_Fim"] = HoraFim ?? "Não informado",
            ["Duracao"] = DuracaoAtendimento?.ToString(@"hh\:mm") ?? "Não calculado",
            ["Empregado"] = Empregado?.NomeEmpregado ?? "Não informado",
            ["Profissional"] = ProfSaude?.NomeProfsaude ?? "Não informado",
            ["CID"] = CodCid,
            ["Diagnostico"] = HipoteseDiagnostica ?? "Não informado",
            ["Status"] = StatusDescricao,
            ["Prioridade"] = PrioridadeDescricao,
            ["Apto_Trabalho"] = AptoTrabalho switch
            {
                "S" => "Sim",
                "N" => "Não",
                "R" => "Restrito",
                _ => "Não avaliado"
            },
            ["Tem_Restricoes"] = TemRestricoes ? "Sim" : "Não",
            ["Afastamento"] = AfastamentoTrabalho ? $"{DiasAfastamento} dias" : "Não",
            ["Retorno_Necessario"] = RetornoNecessario ? "Sim" : "Não",
            ["Data_Retorno"] = DataRetorno?.ToString("dd/MM/yyyy") ?? "Não agendado"
        };
    }

    public override string ToString()
    {
        return $"Atendimento {NumeroProtocolo ?? CodRegistro.ToString()} - {DataAtendimento:dd/MM/yyyy} - {Empregado?.NomeEmpregado ?? "Não informado"}";
    }
}