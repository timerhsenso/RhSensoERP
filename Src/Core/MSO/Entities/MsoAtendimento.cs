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
    public string? DescOcorrencia { get; set; }

    [Column("cod_profsaude")]
    public int? CodProfSaude { get; set; }

    [Column("tipo_atendimento")]
    [StringLength(30)]
    public string? TipoAtendimento { get; set; } = "CONSULTA";

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = "EM_ANDAMENTO";

    [Column("prioridade")]
    [StringLength(10)]
    public string? Prioridade { get; set; } = "NORMAL";

    [Column("queixa_principal")]
    [StringLength(1000)]
    public string? QueixaPrincipal { get; set; }

    [Column("exame_fisico")]
    [StringLength(2000)]
    public string? ExameFisico { get; set; }

    [Column("conduta")]
    [StringLength(1000)]
    public string? Conduta { get; set; }

    [Column("observacoes")]
    [StringLength(2000)]
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
    [ForeignKey(nameof(CodEmpregado))]
    public virtual MsoEmpregado? Empregado { get; set; }

    [ForeignKey(nameof(CodProfSaude))]
    public virtual MsoProfsaude? ProfSaude { get; set; }

    [ForeignKey(nameof(CodCid))]
    public virtual MsoCid? Cid { get; set; }

    public virtual ICollection<MsoAtendimentoItens> AtendimentoItens { get; set; } = new List<MsoAtendimentoItens>();

    // Propriedades calculadas
    [NotMapped]
    public string StatusDescricao => Status switch
    {
        "EM_ANDAMENTO" => "Em Andamento",
        "CONCLUIDO" => "Concluído",
        "CANCELADO" => "Cancelado",
        _ => "Indefinido"
    };

    [NotMapped]
    public bool PodeEditar => Status != "CANCELADO";

    // Métodos de negócio
    public void ConcluirAtendimento(string usuario)
    {
        if (Status != "EM_ANDAMENTO")
            throw new InvalidOperationException("Só é possível concluir atendimentos em andamento.");

        Status = "CONCLUIDO";
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

    public override string ToString()
    {
        return $"Atendimento {CodRegistro} - {DataAtendimento:dd/MM/yyyy} - {Empregado?.NomeEmpregado ?? "N/A"}";
    }
}