// Src/Core/MSO/Entities/MsoConsultaExame.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_consulta_exame")]
public class MsoConsultaExame
{
    [Key, Column("cod_consulta", Order = 1)]
    public int CodConsulta { get; set; }

    [Key, Column("cod_empregado", Order = 2)]
    public int CodEmpregado { get; set; }

    [Key, Column("cod_exame", Order = 3)]
    public int CodExame { get; set; }

    [Column("cod_prestador")]
    public int? CodPrestador { get; set; }

    [Column("data_exame")]
    public DateTime DataExame { get; set; }

    [Column("data_entrega")]
    public DateTime? DataEntrega { get; set; }

    [Column("cod_avaliacao")]
    public int? CodAvaliacao { get; set; }

    [Column("ch_resultado")]
    public int? ChResultado { get; set; }

    [Column("vl_resultado")]
    public decimal? VlResultado { get; set; }

    [Column("desc_resultado")]
    [StringLength(255)]
    public string? DescResultado { get; set; }

    [Column("status_exame")]
    [StringLength(20)]
    public string StatusExame { get; set; } = "SOLICITADO";

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
    [ForeignKey(nameof(CodConsulta))]
    public virtual MsoConsultaGeral? ConsultaGeral { get; set; }

    [ForeignKey(nameof(CodEmpregado))]
    public virtual MsoEmpregado? Empregado { get; set; }

    [ForeignKey(nameof(CodExame))]
    public virtual MsoExameAux? ExameAux { get; set; }

    [ForeignKey(nameof(CodPrestador))]
    public virtual MsoExameprestador? Prestador { get; set; }

    [ForeignKey(nameof(CodAvaliacao))]
    public virtual MsoAvaliacao? Avaliacao { get; set; }

    // Propriedades calculadas
    [NotMapped]
    public string StatusDescricao => StatusExame switch
    {
        "SOLICITADO" => "Solicitado",
        "AGENDADO" => "Agendado",
        "EXECUTADO" => "Executado",
        "RESULTADO_DISPONIVEL" => "Resultado Disponível",
        "ENTREGUE" => "Entregue",
        "CANCELADO" => "Cancelado",
        _ => "Indefinido"
    };

    [NotMapped]
    public bool TemResultado => !string.IsNullOrWhiteSpace(DescResultado) || VlResultado.HasValue;

    [NotMapped]
    public bool JaEntregue => DataEntrega.HasValue;

    [NotMapped]
    public int DiasParaEntrega
    {
        get
        {
            if (!DataEntrega.HasValue) return 0;
            return (DataEntrega.Value.Date - DateTime.Today).Days;
        }
    }

    [NotMapped]
    public bool Atrasado => DataEntrega.HasValue && DataEntrega.Value.Date < DateTime.Today && StatusExame != "ENTREGUE";

    // Métodos de negócio
    public void RegistrarResultado(decimal? valorResultado, string descricaoResultado, string usuario)
    {
        VlResultado = valorResultado;
        DescResultado = descricaoResultado;
        StatusExame = "RESULTADO_DISPONIVEL";
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void MarcarComoEntregue(string usuario)
    {
        if (StatusExame != "RESULTADO_DISPONIVEL")
            throw new InvalidOperationException("Só é possível entregar exames com resultado disponível.");

        StatusExame = "ENTREGUE";
        DataEntrega = DateTime.Now;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void CancelarExame(string usuario, string motivo = null)
    {
        StatusExame = "CANCELADO";

        if (!string.IsNullOrEmpty(motivo))
            Observacoes = $"{Observacoes}\nCancelado: {motivo}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public override string ToString()
    {
        return $"Exame {CodExame} - Consulta {CodConsulta} - {StatusDescricao}";
    }
}