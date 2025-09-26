// Src/Core/MSO/Entities/Notas.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_notas")]
public class Notas
{
    [Key]
    [Column("cod_nota")]
    public int CodNota { get; set; }

    [Column("cod_empregado")]
    public int CodEmpregado { get; set; }

    [Column("data_nota")]
    public DateTime DataNota { get; set; }

    [Column("titulo")]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Column("conteudo")]
    [StringLength(4000)]
    public string Conteudo { get; set; } = string.Empty;

    [Column("tipo_nota")]
    [StringLength(20)]
    public string? TipoNota { get; set; } // OBSERVACAO, LEMBRETE, ALERTA

    [Column("prioridade")]
    [StringLength(10)]
    public string? Prioridade { get; set; } // BAIXA, MEDIA, ALTA

    [Column("cod_usuario")]
    [StringLength(20)]
    public string? CodUsuario { get; set; }

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Empregado? Empregado { get; set; }
}
