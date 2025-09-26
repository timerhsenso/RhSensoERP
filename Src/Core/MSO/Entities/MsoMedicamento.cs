// Src/Core/MSO/Entities/Medicamento.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_medicamento")]
public class Medicamento
{
    [Key]
    [Column("cod_medicamento")]
    public int CodMedicamento { get; set; }

    [Column("nome_medicamento")]
    [StringLength(200)]
    public string NomeMedicamento { get; set; } = string.Empty;

    [Column("nome_generico")]
    [StringLength(200)]
    public string? NomeGenerico { get; set; }

    [Column("principio_ativo")]
    [StringLength(200)]
    public string? PrincipioAtivo { get; set; }

    [Column("forma_farmaceutica")]
    [StringLength(50)]
    public string? FormaFarmaceutica { get; set; }

    [Column("concentracao")]
    [StringLength(50)]
    public string? Concentracao { get; set; }

    [Column("laboratorio")]
    [StringLength(100)]
    public string? Laboratorio { get; set; }

    [Column("codigo_eans")]
    [StringLength(20)]
    public string? CodigoEans { get; set; }

    [Column("tarja")]
    [StringLength(20)]
    public string? Tarja { get; set; }

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ICollection<AtendimentoMedicamento> AtendimentoMedicamentos { get; set; } = new List<AtendimentoMedicamento>();
}