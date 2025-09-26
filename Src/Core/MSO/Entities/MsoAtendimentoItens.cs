// Src/Core/MSO/Entities/MsoAtendimentoItens.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_atendimento_itens")]
public class MsoAtendimentoItens
{
    [Key, Column("cod_empregado", Order = 1)]
    public int CodEmpregado { get; set; }

    [Key, Column("data_atendimento", Order = 2)]
    public DateTime DataAtendimento { get; set; }

    [Key, Column("cod_tpconsulta", Order = 3)]
    public int CodTpConsulta { get; set; }

    [Key, Column("cod_registro", Order = 4)]
    public int CodRegistro { get; set; }

    [Column("sequencial")]
    public int? Sequencial { get; set; }

    [Column("descricao_item")]
    [StringLength(500)]
    public string? DescricaoItem { get; set; }

    [Column("tipo_item")]
    [StringLength(50)]
    public string? TipoItem { get; set; } // PROCEDIMENTO, EXAME, MEDICAMENTO, ORIENTACAO

    [Column("status_item")]
    [StringLength(20)]
    public string StatusItem { get; set; } = "PENDENTE"; // PENDENTE, EXECUTADO, CANCELADO

    [Column("resultado")]
    [StringLength(2000)]
    public string? Resultado { get; set; }

    [Column("valor_numerico")]
    public decimal? ValorNumerico { get; set; }

    [Column("unidade_medida")]
    [StringLength(20)]
    public string? UnidadeMedida { get; set; }

    [Column("valor_referencia")]
    [StringLength(100)]
    public string? ValorReferencia { get; set; }

    [Column("alterado")]
    [StringLength(1)]
    public string? Alterado { get; set; } // S/N

    [Column("critico")]
    [StringLength(1)]
    public string? Critico { get; set; } // S/N

    [Column("cod_prestador")]
    public int? CodPrestador { get; set; }

    [Column("data_execucao")]
    public DateTime? DataExecucao { get; set; }

    [Column("hora_execucao")]
    [StringLength(5)]
    public string? HoraExecucao { get; set; }

    [Column("cod_profsaude_executor")]
    public int? CodProfSaudeExecutor { get; set; }

    [Column("observacoes")]
    [StringLength(1000)]
    public string? Observacoes { get; set; }

    [Column("prioridade")]
    [StringLength(10)]
    public string? Prioridade { get; set; } = "NORMAL"; // BAIXA, NORMAL, ALTA, URGENTE

    [Column("tempo_execucao_minutos")]
    public int? TempoExecucaoMinutos { get; set; }

    [Column("custo_estimado")]
    public decimal? CustoEstimado { get; set; }

    [Column("requer_jejum")]
    public bool RequerJejum { get; set; } = false;

    [Column("orientacoes_preparo")]
    [StringLength(1000)]
    public string? OrientacoesPreparo { get; set; }

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

    [Column("protocolo_item")]
    [StringLength(30)]
    public string? ProtocoloItem { get; set; }

    [Column("arquivo_resultado")]
    [StringLength(500)]
    public string? ArquivoResultado { get; set; }

    [Column("data_entrega_resultado")]
    public DateTime? DataEntregaResultado { get; set; }

    [Column("validado_por")]
    [StringLength(50)]
    public string? ValidadoPor { get; set; }

    [Column("data_validacao")]
    public DateTime? DataValidacao { get; set; }

    // Relacionamentos
    [ForeignKey($"{nameof(CodEmpregado)},{nameof(DataAtendimento)},{nameof(CodRegistro)}")]
    public virtual MsoAtendimento? Atendimento { get; set; }

    [ForeignKey(nameof(CodTpConsulta))]
    public virtual MsoTpconsulta? TpConsulta { get; set; }

    [ForeignKey(nameof(CodPrestador))]
    public virtual MsoExameprestador? Prestador { get; set; }

    [ForeignKey(nameof(CodProfSaudeExecutor))]
    public virtual MsoProfsaude? ProfSaudeExecutor { get; set; }

    // Propriedades calculadas
    [NotMapped]
    public DateTime? DataHoraExecucao
    {
        get
        {
            if (DataExecucao.HasValue && !string.IsNullOrEmpty(HoraExecucao))
            {
                if (TimeSpan.TryParse(HoraExecucao, out var hora))
                    return DataExecucao.Value.Date.Add(hora);
            }
            return DataExecucao;
        }
    }

    [NotMapped]
    public string StatusDescricao => StatusItem switch
    {
        "PENDENTE" => "Pendente",
        "AGENDADO" => "Agendado",
        "EXECUTADO" => "Executado",
        "CANCELADO" => "Cancelado",
        "AGUARDANDO_RESULTADO" => "Aguardando Resultado",
        "RESULTADO_DISPONIVEL" => "Resultado Disponível",
        "VALIDADO" => "Validado",
        _ => "Indefinido"
    };

    [NotMapped]
    public string PrioridadeDescricao => Prioridade switch
    {
        "BAIXA" => "Baixa",
        "NORMAL" => "Normal",
        "ALTA" => "Alta",
        "URGENTE" => "Urgente",
        "STAT" => "STAT",
        _ => "Normal"
    };

    [NotMapped]
    public string TipoItemDescricao => TipoItem switch
    {
        "PROCEDIMENTO" => "Procedimento",
        "EXAME" => "Exame",
        "MEDICAMENTO" => "Medicamento",
        "ORIENTACAO" => "Orientação",
        "VACINA" => "Vacina",
        "CURATIVO" => "Curativo",
        "COLETA" => "Coleta",
        _ => "Não especificado"
    };

    [NotMapped]
    public bool PodeExecutar => StatusItem is "PENDENTE" or "AGENDADO";

    [NotMapped]
    public bool PodeCancelar => StatusItem != "EXECUTADO" && StatusItem != "VALIDADO";

    [NotMapped]
    public bool PodeEditar => StatusItem != "EXECUTADO" && StatusItem != "VALIDADO" && StatusItem != "CANCELADO";

    [NotMapped]
    public bool TemResultado => !string.IsNullOrWhiteSpace(Resultado) || ValorNumerico.HasValue;

    [NotMapped]
    public bool ResultadoAlterado => Alterado == "S";

    [NotMapped]
    public bool ResultadoCritico => Critico == "S";

    [NotMapped]
    public bool JaExecutado => StatusItem is "EXECUTADO" or "RESULTADO_DISPONIVEL" or "VALIDADO";

    [NotMapped]
    public bool TemArquivo => !string.IsNullOrWhiteSpace(ArquivoResultado);

    [NotMapped]
    public bool JaValidado => !string.IsNullOrWhiteSpace(ValidadoPor) && DataValidacao.HasValue;

    [NotMapped]
    public bool PrecisaJejum => RequerJejum;

    [NotMapped]
    public bool TemOrientacoes => !string.IsNullOrWhiteSpace(OrientacoesPreparo);

    [NotMapped]
    public TimeSpan? TempoDecorrido
    {
        get
        {
            if (DataHoraExecucao.HasValue && TempoExecucaoMinutos.HasValue)
                return TimeSpan.FromMinutes(TempoExecucaoMinutos.Value);
            return null;
        }
    }

    [NotMapped]
    public int DiasAteEntrega
    {
        get
        {
            if (DataEntregaResultado.HasValue)
                return (DataEntregaResultado.Value.Date - DateTime.Today).Days;
            return 0;
        }
    }

    [NotMapped]
    public bool ResultadoAtrasado => DataEntregaResultado.HasValue && DataEntregaResultado.Value.Date < DateTime.Today && StatusItem != "VALIDADO";

    // Métodos de negócio
    public void IniciarExecucao(string usuario, int? codProfExecutor = null)
    {
        if (!PodeExecutar)
            throw new InvalidOperationException($"Item com status '{StatusItem}' não pode ser executado.");

        StatusItem = "EXECUTADO";
        DataExecucao = DateTime.Now;
        HoraExecucao = DateTime.Now.ToString("HH:mm");
        CodProfSaudeExecutor = codProfExecutor;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void RegistrarResultado(string resultado, string usuario, decimal? valorNumerico = null, string unidade = null)
    {
        if (StatusItem == "CANCELADO")
            throw new InvalidOperationException("Não é possível registrar resultado para item cancelado.");

        Resultado = resultado;
        ValorNumerico = valorNumerico;
        UnidadeMedida = unidade;
        StatusItem = "RESULTADO_DISPONIVEL";
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;

        VerificarSeResultadoAlterado();
    }

    public void CancelarItem(string usuario, string motivo = null)
    {
        if (!PodeCancelar)
            throw new InvalidOperationException($"Item com status '{StatusItem}' não pode ser cancelado.");

        StatusItem = "CANCELADO";

        if (!string.IsNullOrEmpty(motivo))
            Observacoes = $"{Observacoes}\nCancelado: {motivo}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AgendarExecucao(DateTime dataAgendamento, string hora, string usuario)
    {
        if (StatusItem != "PENDENTE")
            throw new InvalidOperationException("Só é possível agendar itens pendentes.");

        StatusItem = "AGENDADO";
        DataExecucao = dataAgendamento;
        HoraExecucao = hora;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void ValidarResultado(string validadorUsuario, string observacoesValidacao = null)
    {
        if (StatusItem != "RESULTADO_DISPONIVEL")
            throw new InvalidOperationException("Só é possível validar itens com resultado disponível.");

        StatusItem = "VALIDADO";
        ValidadoPor = validadorUsuario;
        DataValidacao = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(observacoesValidacao))
            Observacoes = $"{Observacoes}\nValidação: {observacoesValidacao}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = validadorUsuario;
    }

    public void AnexarArquivo(string caminhoArquivo, string usuario)
    {
        if (string.IsNullOrWhiteSpace(caminhoArquivo))
            throw new ArgumentException("Caminho do arquivo não pode estar vazio.");

        ArquivoResultado = caminhoArquivo;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void DefinirPrazoEntrega(DateTime dataEntrega, string usuario)
    {
        DataEntregaResultado = dataEntrega;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AlterarPrioridade(string novaPrioridade, string usuario, string justificativa = null)
    {
        if (string.IsNullOrWhiteSpace(novaPrioridade))
            throw new ArgumentException("Prioridade não pode estar vazia.");

        Prioridade = novaPrioridade.ToUpper();

        if (!string.IsNullOrEmpty(justificativa))
            Observacoes = $"{Observacoes}\nPrioridade alterada para {novaPrioridade}: {justificativa}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void GerarProtocolo()
    {
        if (string.IsNullOrEmpty(ProtocoloItem))
        {
            var ano = DataAtendimento.Year;
            var mes = DataAtendimento.Month.ToString("D2");
            var dia = DataAtendimento.Day.ToString("D2");
            var sequencial = (Sequencial ?? CodRegistro).ToString("D4");
            var tipoAbrev = TipoItem?.Substring(0, Math.Min(2, TipoItem.Length)) ?? "IT";

            ProtocoloItem = $"{tipoAbrev}{ano}{mes}{dia}{sequencial}";
        }
    }

    // Validações
    private void VerificarSeResultadoAlterado()
    {
        if (ValorNumerico.HasValue && !string.IsNullOrWhiteSpace(ValorReferencia))
        {
            // Lógica simplificada - em produção seria mais complexa
            if (ValorReferencia.Contains("-"))
            {
                var partes = ValorReferencia.Split('-');
                if (partes.Length == 2 &&
                    decimal.TryParse(partes[0].Trim(), out var min) &&
                    decimal.TryParse(partes[1].Trim(), out var max))
                {
                    Alterado = (ValorNumerico < min || ValorNumerico > max) ? "S" : "N";

                    // Verificar se é crítico (muito fora dos limites)
                    var faixaCritica = (max - min) * 0.5m; // 50% fora da faixa
                    Critico = (ValorNumerico < (min - faixaCritica) || ValorNumerico > (max + faixaCritica)) ? "S" : "N";
                }
            }
        }
    }

    public bool IsValorReferenciaValida()
    {
        return !string.IsNullOrWhiteSpace(ValorReferencia);
    }

    public bool IsResultadoValido()
    {
        return !string.IsNullOrWhiteSpace(Resultado) || ValorNumerico.HasValue;
    }

    public bool IsDataExecucaoValida()
    {
        return !DataExecucao.HasValue || DataExecucao.Value.Date >= DataAtendimento.Date;
    }

    public bool IsHoraExecucaoValida()
    {
        return string.IsNullOrEmpty(HoraExecucao) || TimeSpan.TryParse(HoraExecucao, out _);
    }

    public bool IsPrioridadeValida()
    {
        var prioridadesValidas = new[] { "BAIXA", "NORMAL", "ALTA", "URGENTE", "STAT" };
        return string.IsNullOrEmpty(Prioridade) || prioridadesValidas.Contains(Prioridade.ToUpper());
    }

    // Métodos de relatório
    public Dictionary<string, object> ToResumoItem()
    {
        return new Dictionary<string, object>
        {
            ["Protocolo"] = ProtocoloItem ?? "Não gerado",
            ["Tipo"] = TipoItemDescricao,
            ["Descricao"] = DescricaoItem ?? "Não informado",
            ["Status"] = StatusDescricao,
            ["Prioridade"] = PrioridadeDescricao,
            ["Data_Execucao"] = DataExecucao?.ToString("dd/MM/yyyy") ?? "Não executado",
            ["Hora_Execucao"] = HoraExecucao ?? "Não informado",
            ["Resultado"] = Resultado ?? "Sem resultado",
            ["Valor_Numerico"] = ValorNumerico?.ToString() ?? "N/A",
            ["Unidade"] = UnidadeMedida ?? "N/A",
            ["Alterado"] = ResultadoAlterado ? "Sim" : "Não",
            ["Critico"] = ResultadoCritico ? "Sim" : "Não",
            ["Validado"] = JaValidado ? "Sim" : "Não",
            ["Prestador"] = Prestador?.NomePrestador ?? "Não informado",
            ["Profissional_Executor"] = ProfSaudeExecutor?.NomeProfsaude ?? "Não informado",
            ["Custo_Estimado"] = CustoEstimado?.ToString("C") ?? "Não informado",
            ["Tempo_Execucao"] = TempoExecucaoMinutos?.ToString() + " min" ?? "Não informado"
        };
    }

    public override string ToString()
    {
        return $"{TipoItemDescricao}: {DescricaoItem ?? "Sem descrição"} - {StatusDescricao}";
    }
}