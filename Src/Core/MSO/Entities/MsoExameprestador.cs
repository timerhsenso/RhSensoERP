// Src/Core/MSO/Entities/MsoExameprestador.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_exameprestador")]
public class MsoExameprestador
{
    [Key, Column("cod_prestador", Order = 1)]
    public int CodPrestador { get; set; }

    [Key, Column("cod_exame", Order = 2)]
    public int CodExame { get; set; }

    [Column("metodo_coleta")]
    [StringLength(200)]
    public string? MetodoColeta { get; set; }

    [Column("obs_exame")]
    [StringLength(200)]
    public string? ObsExame { get; set; }

    [Column("ch_padrao")]
    [StringLength(1)]
    public string? ChPadrao { get; set; } = "N";

    [Column("valor_exame")]
    public decimal? ValorExame { get; set; }

    [Column("tempo_execucao_dias")]
    public int? TempoExecucaoDias { get; set; }

    [Column("tempo_resultado_dias")]
    public int? TempoResultadoDias { get; set; }

    [Column("requer_agendamento")]
    public bool RequerAgendamento { get; set; } = false;

    [Column("requer_jejum")]
    public bool RequerJejum { get; set; } = false;

    [Column("tempo_jejum_horas")]
    public int? TempoJejumHoras { get; set; }

    [Column("orientacoes_preparo")]
    [StringLength(1000)]
    public string? OrientacoesPreparo { get; set; }

    [Column("material_coleta")]
    [StringLength(100)]
    public string? MaterialColeta { get; set; }

    [Column("volume_amostra")]
    [StringLength(50)]
    public string? VolumeAmostra { get; set; }

    [Column("condicoes_transporte")]
    [StringLength(200)]
    public string? CondicoesTransporte { get; set; }

    [Column("temperatura_armazenamento")]
    [StringLength(50)]
    public string? TemperaturaArmazenamento { get; set; }

    [Column("prazo_validade_amostra")]
    public int? PrazoValidadeAmostra { get; set; } // Em horas

    [Column("codigo_exame_prestador")]
    [StringLength(20)]
    public string? CodigoExamePrestador { get; set; }

    [Column("disponivel")]
    public bool Disponivel { get; set; } = true;

    [Column("prioridade")]
    public int? Prioridade { get; set; } = 1; // 1 = Preferencial

    [Column("observacoes_internas")]
    [StringLength(1000)]
    public string? ObservacoesInternas { get; set; }

    [Column("contato_responsavel")]
    [StringLength(100)]
    public string? ContatoResponsavel { get; set; }

    [Column("telefone_contato")]
    [StringLength(20)]
    public string? TelefoneContato { get; set; }

    [Column("email_contato")]
    [StringLength(100)]
    public string? EmailContato { get; set; }

    [Column("horario_funcionamento")]
    [StringLength(100)]
    public string? HorarioFuncionamento { get; set; }

    [Column("dias_funcionamento")]
    [StringLength(50)]
    public string? DiasFuncionamento { get; set; } // SEG-SEX, SAB, DOM

    [Column("endereco_coleta")]
    [StringLength(200)]
    public string? EnderecoColeta { get; set; }

    [Column("coleta_domiciliar")]
    public bool ColetaDomiciliar { get; set; } = false;

    [Column("valor_coleta_domiciliar")]
    public decimal? ValorColetaDomiciliar { get; set; }

    [Column("data_inicio_vigencia")]
    public DateTime? DataInicioVigencia { get; set; }

    [Column("data_fim_vigencia")]
    public DateTime? DataFimVigencia { get; set; }

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

    [Column("numero_contrato")]
    [StringLength(30)]
    public string? NumeroContrato { get; set; }

    [Column("data_contrato")]
    public DateTime? DataContrato { get; set; }

    [Column("data_vencimento_contrato")]
    public DateTime? DataVencimentoContrato { get; set; }

    // Relacionamentos
    [ForeignKey(nameof(CodPrestador))]
    public virtual MsoExameprestador? Prestador { get; set; }

    [ForeignKey(nameof(CodExame))]
    public virtual MsoExameAux? Exame { get; set; }

    public virtual ICollection<MsoAtendimentoItens> AtendimentoItens { get; set; } = new List<MsoAtendimentoItens>();
    public virtual ICollection<MsoResultadoExame> ResultadosExame { get; set; } = new List<MsoResultadoExame>();

    // Propriedades calculadas
    [NotMapped]
    public bool IsPadrao => ChPadrao == "S";

    [NotMapped]
    public bool PrecisaJejum => RequerJejum;

    [NotMapped]
    public bool TemColeta => !string.IsNullOrWhiteSpace(MaterialColeta);

    [NotMapped]
    public bool FazColetaDomiciliar => ColetaDomiciliar;

    [NotMapped]
    public bool EstaDisponivel => Disponivel && Ativo && EstaVigente;

    [NotMapped]
    public bool EstaVigente
    {
        get
        {
            var hoje = DateTime.Today;
            var inicioOk = !DataInicioVigencia.HasValue || DataInicioVigencia.Value.Date <= hoje;
            var fimOk = !DataFimVigencia.HasValue || DataFimVigencia.Value.Date >= hoje;
            return inicioOk && fimOk;
        }
    }

    [NotMapped]
    public bool ContratoVigente
    {
        get
        {
            if (!DataContrato.HasValue) return true; // Sem contrato = sempre vigente

            var hoje = DateTime.Today;
            var contratoOk = DataContrato.Value.Date <= hoje;
            var vencimentoOk = !DataVencimentoContrato.HasValue || DataVencimentoContrato.Value.Date >= hoje;

            return contratoOk && vencimentoOk;
        }
    }

    [NotMapped]
    public int DiasParaResultado => TempoResultadoDias ?? TempoExecucaoDias ?? 1;

    [NotMapped]
    public DateTime? DataPrevistaResultado
    {
        get
        {
            if (DiasParaResultado > 0)
                return DateTime.Today.AddDays(DiasParaResultado);
            return null;
        }
    }

    [NotMapped]
    public decimal ValorTotal
    {
        get
        {
            var valor = ValorExame ?? 0;
            if (ColetaDomiciliar && ValorColetaDomiciliar.HasValue)
                valor += ValorColetaDomiciliar.Value;
            return valor;
        }
    }

    [NotMapped]
    public string DescricaoJejum
    {
        get
        {
            if (!RequerJejum) return "Não requer jejum";
            if (TempoJejumHoras.HasValue)
                return $"Jejum de {TempoJejumHoras}h";
            return "Jejum necessário";
        }
    }

    [NotMapped]
    public string StatusVigencia
    {
        get
        {
            if (!EstaVigente) return "Fora de vigência";
            if (!ContratoVigente) return "Contrato vencido";
            if (!Disponivel) return "Indisponível";
            if (!Ativo) return "Inativo";
            return "Disponível";
        }
    }

    [NotMapped]
    public string PrioridadeDescricao => Prioridade switch
    {
        1 => "Preferencial",
        2 => "Secundário",
        3 => "Reserva",
        _ => "Não definido"
    };

    [NotMapped]
    public int DiasParaVencimentoContrato
    {
        get
        {
            if (!DataVencimentoContrato.HasValue) return int.MaxValue;
            return (DataVencimentoContrato.Value.Date - DateTime.Today).Days;
        }
    }

    [NotMapped]
    public bool ContratoProximoVencimento => DiasParaVencimentoContrato <= 30 && DiasParaVencimentoContrato > 0;

    // Métodos de negócio
    public void DefinirComoPadrao(string usuario)
    {
        ChPadrao = "S";
        Prioridade = 1;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void RemoverPadrao(string usuario)
    {
        ChPadrao = "N";
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AlterarDisponibilidade(bool disponivel, string usuario, string motivo = null)
    {
        Disponivel = disponivel;

        if (!string.IsNullOrEmpty(motivo))
            ObservacoesInternas = $"{ObservacoesInternas}\n{DateTime.Now:dd/MM/yyyy}: {(disponivel ? "Disponibilizado" : "Indisponibilizado")} - {motivo}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AtualizarValor(decimal novoValor, string usuario, string justificativa = null)
    {
        var valorAnterior = ValorExame;
        ValorExame = novoValor;

        if (!string.IsNullOrEmpty(justificativa))
            ObservacoesInternas = $"{ObservacoesInternas}\n{DateTime.Now:dd/MM/yyyy}: Valor alterado de {valorAnterior:C} para {novoValor:C} - {justificativa}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void DefinirVigencia(DateTime inicio, DateTime? fim, string usuario)
    {
        DataInicioVigencia = inicio;
        DataFimVigencia = fim;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void RenovarContrato(DateTime novaDataVencimento, string novoNumeroContrato, string usuario)
    {
        var contratoAnterior = NumeroContrato;
        var vencimentoAnterior = DataVencimentoContrato;

        NumeroContrato = novoNumeroContrato;
        DataContrato = DateTime.Today;
        DataVencimentoContrato = novaDataVencimento;

        ObservacoesInternas = $"{ObservacoesInternas}\n{DateTime.Now:dd/MM/yyyy}: Contrato renovado. Anterior: {contratoAnterior} (venc: {vencimentoAnterior:dd/MM/yyyy}) → Novo: {novoNumeroContrato} (venc: {novaDataVencimento:dd/MM/yyyy})".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AlterarPrioridade(int novaPrioridade, string usuario, string motivo = null)
    {
        if (novaPrioridade < 1 || novaPrioridade > 10)
            throw new ArgumentException("Prioridade deve estar entre 1 e 10.");

        var prioridadeAnterior = Prioridade;
        Prioridade = novaPrioridade;

        if (!string.IsNullOrEmpty(motivo))
            ObservacoesInternas = $"{ObservacoesInternas}\n{DateTime.Now:dd/MM/yyyy}: Prioridade alterada de {prioridadeAnterior} para {novaPrioridade} - {motivo}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AtualizarTempoExecucao(int diasExecucao, int? diasResultado, string usuario)
    {
        TempoExecucaoDias = diasExecucao;
        TempoResultadoDias = diasResultado;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void DefinirColetaDomiciliar(bool oferece, decimal? valorColeta, string usuario)
    {
        ColetaDomiciliar = oferece;
        ValorColetaDomiciliar = valorColeta;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AtualizarContato(string responsavel, string telefone, string email, string usuario)
    {
        ContatoResponsavel = responsavel;
        TelefoneContato = telefone;
        EmailContato = email;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    // Validações
    public bool IsVigenciaValida()
    {
        if (!DataInicioVigencia.HasValue) return true;
        if (!DataFimVigencia.HasValue) return true;

        return DataFimVigencia.Value >= DataInicioVigencia.Value;
    }

    public bool IsContratoValido()
    {
        if (!DataContrato.HasValue) return true;
        if (!DataVencimentoContrato.HasValue) return true;

        return DataVencimentoContrato.Value >= DataContrato.Value;
    }

    public bool IsValorValido()
    {
        return !ValorExame.HasValue || ValorExame.Value >= 0;
    }

    public bool IsTempoExecucaoValido()
    {
        return !TempoExecucaoDias.HasValue || TempoExecucaoDias.Value > 0;
    }

    public bool IsTempoJejumValido()
    {
        return !TempoJejumHoras.HasValue || TempoJejumHoras.Value > 0;
    }

    public bool TemInformacoesCompletas()
    {
        return !string.IsNullOrWhiteSpace(MetodoColeta) &&
               ValorExame.HasValue &&
               TempoExecucaoDias.HasValue;
    }

    // Métodos de comparação
    public int CompararPor(MsoExameprestador outro, string criterio = "prioridade")
    {
        return criterio.ToLower() switch
        {
            "prioridade" => (Prioridade ?? 999).CompareTo(outro.Prioridade ?? 999),
            "valor" => (ValorTotal).CompareTo(outro.ValorTotal),
            "tempo" => (DiasParaResultado).CompareTo(outro.DiasParaResultado),
            "padrao" => (outro.IsPadrao).CompareTo(IsPadrao), // Padrão primeiro
            _ => string.Compare(MetodoColeta, outro.MetodoColeta, StringComparison.OrdinalIgnoreCase)
        };
    }

    // Métodos de relatório
    public Dictionary<string, object> ToRelatorioPrestador()
    {
        return new Dictionary<string, object>
        {
            ["Codigo_Prestador"] = CodPrestador,
            ["Codigo_Exame"] = CodExame,
            ["Metodo_Coleta"] = MetodoColeta ?? "Não informado",
            ["Valor_Exame"] = ValorExame?.ToString("C") ?? "Não informado",
            ["Valor_Total"] = ValorTotal.ToString("C"),
            ["Tempo_Execucao"] = $"{TempoExecucaoDias ?? 0} dias",
            ["Tempo_Resultado"] = $"{DiasParaResultado} dias",
            ["Padrao"] = IsPadrao ? "Sim" : "Não",
            ["Prioridade"] = PrioridadeDescricao,
            ["Status"] = StatusVigencia,
            ["Requer_Jejum"] = DescricaoJejum,
            ["Coleta_Domiciliar"] = ColetaDomiciliar ? "Disponível" : "Não disponível",
            ["Valor_Coleta_Domiciliar"] = ValorColetaDomiciliar?.ToString("C") ?? "N/A",
            ["Disponivel"] = Disponivel ? "Sim" : "Não",
            ["Vigencia"] = EstaVigente ? "Vigente" : "Fora de vigência",
            ["Contrato"] = NumeroContrato ?? "Não informado",
            ["Vencimento_Contrato"] = DataVencimentoContrato?.ToString("dd/MM/yyyy") ?? "Não informado",
            ["Dias_Para_Vencimento"] = DiasParaVencimentoContrato == int.MaxValue ? "Sem vencimento" : $"{DiasParaVencimentoContrato} dias",
            ["Contato"] = ContatoResponsavel ?? "Não informado",
            ["Telefone"] = TelefoneContato ?? "Não informado",
            ["Email"] = EmailContato ?? "Não informado"
        };
    }

    public override string ToString()
    {
        var status = IsPadrao ? " (PADRÃO)" : "";
        var disponivel = EstaDisponivel ? "" : " [INDISPONÍVEL]";
        return $"Prestador {CodPrestador} - Exame {CodExame} - {ValorTotal:C}{status}{disponivel}";
    }
}