// Src/Core/MSO/Entities/MsoAgenda.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_agenda")]
public class MsoAgenda
{
    [Key, Column("cod_profsaude", Order = 1)]
    public int CodProfSaude { get; set; }

    [Key, Column("cod_empregado", Order = 2)]
    public int CodEmpregado { get; set; }

    [Key, Column("data_consulta", Order = 3)]
    public DateTime DataConsulta { get; set; }

    [Key, Column("hora_inicio", Order = 4)]
    [StringLength(5)]
    public string HoraInicio { get; set; } = string.Empty;

    [Column("cod_solicitacao")]
    public int? CodSolicitacao { get; set; }

    [Column("cod_consulta")]
    public int? CodConsulta { get; set; }

    [Column("cod_tpconsulta")]
    public int? CodTpConsulta { get; set; }

    [Column("hora_fim")]
    [StringLength(5)]
    public string? HoraFim { get; set; }

    [Column("ch_atendimento")]
    [Required]
    public string ChAtendimento { get; set; } = string.Empty;

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = "AGENDADO";

    [Column("observacoes")]
    [StringLength(1000)]
    public string? Observacoes { get; set; }

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

    // Relacionamentos
    [ForeignKey(nameof(CodProfSaude))]
    public virtual MsoProfsaude? ProfSaude { get; set; }

    [ForeignKey(nameof(CodEmpregado))]
    public virtual MsoEmpregado? Empregado { get; set; }

    [ForeignKey(nameof(CodTpConsulta))]
    public virtual MsoTpconsulta? TpConsulta { get; set; }

    [ForeignKey(nameof(CodSolicitacao))]
    public virtual MsoSolicitacao? Solicitacao { get; set; }

    // Propriedades calculadas
    [NotMapped]
    public TimeSpan? DuracaoConsulta
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
                return DataConsulta.Date.Add(hora);
            return DataConsulta;
        }
    }

    [NotMapped]
    public DateTime? DataHoraFim
    {
        get
        {
            if (!string.IsNullOrEmpty(HoraFim) && TimeSpan.TryParse(HoraFim, out var hora))
                return DataConsulta.Date.Add(hora);
            return null;
        }
    }

    [NotMapped]
    public string StatusDescricao => Status switch
    {
        "AGENDADO" => "Agendado",
        "CONFIRMADO" => "Confirmado",
        "EM_ANDAMENTO" => "Em Andamento",
        "CONCLUIDO" => "Concluído",
        "CANCELADO" => "Cancelado",
        "REAGENDADO" => "Reagendado",
        "FALTA" => "Falta do Paciente",
        _ => "Indefinido"
    };

    [NotMapped]
    public bool PodeEditar => Status is "AGENDADO" or "CONFIRMADO";

    [NotMapped]
    public bool PodeCancelar => Status is "AGENDADO" or "CONFIRMADO";

    [NotMapped]
    public bool JaRealizado => Status is "CONCLUIDO";

    // Métodos de negócio
    public void Confirmar(string usuario)
    {
        if (Status != "AGENDADO")
            throw new InvalidOperationException("Só é possível confirmar agendamentos com status 'AGENDADO'.");

        Status = "CONFIRMADO";
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void IniciarAtendimento(string usuario)
    {
        if (Status != "CONFIRMADO" && Status != "AGENDADO")
            throw new InvalidOperationException("Só é possível iniciar atendimento para agendamentos confirmados ou agendados.");

        Status = "EM_ANDAMENTO";
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void ConcluirAtendimento(string usuario)
    {
        if (Status != "EM_ANDAMENTO")
            throw new InvalidOperationException("Só é possível concluir atendimentos em andamento.");

        Status = "CONCLUIDO";
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void Cancelar(string usuario, string motivo = null)
    {
        if (Status == "CONCLUIDO")
            throw new InvalidOperationException("Não é possível cancelar atendimentos já concluídos.");

        Status = "CANCELADO";
        if (!string.IsNullOrEmpty(motivo))
            Observacoes = $"{Observacoes}\nCancelado: {motivo}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void Reagendar(DateTime novaData, string novaHora, string usuario, string motivo = null)
    {
        if (Status == "CONCLUIDO")
            throw new InvalidOperationException("Não é possível reagendar atendimentos já concluídos.");

        DataConsulta = novaData;
        HoraInicio = novaHora;
        Status = "REAGENDADO";

        if (!string.IsNullOrEmpty(motivo))
            Observacoes = $"{Observacoes}\nReagendado: {motivo}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void RegistrarFalta(string usuario, string observacao = null)
    {
        if (Status != "CONFIRMADO" && Status != "AGENDADO")
            throw new InvalidOperationException("Só é possível registrar falta para agendamentos confirmados ou agendados.");

        Status = "FALTA";

        if (!string.IsNullOrEmpty(observacao))
            Observacoes = $"{Observacoes}\nFalta registrada: {observacao}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    // Validações de negócio
    public bool IsHorarioValido()
    {
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

    public bool IsDataValida()
    {
        return DataConsulta.Date >= DateTime.Today;
    }

    public bool TemConflito(IEnumerable<MsoAgenda> outrosAgendamentos)
    {
        if (!IsHorarioValido()) return false;

        var inicioThis = DataHoraInicio;
        var fimThis = DataHoraFim ?? inicioThis.AddMinutes(30); // Assume 30min se não tiver fim

        return outrosAgendamentos.Any(a =>
            a.CodProfSaude == CodProfSaude &&
            a.DataConsulta.Date == DataConsulta.Date &&
            a.Status != "CANCELADO" &&
            !(a.CodEmpregado == CodEmpregado &&
              a.DataConsulta == DataConsulta &&
              a.HoraInicio == HoraInicio) && // Não comparar consigo mesmo
            a.IsHorarioValido() &&
            a.DataHoraInicio < fimThis &&
            (a.DataHoraFim ?? a.DataHoraInicio.AddMinutes(30)) > inicioThis
        );
    }
}