// Src/Core/MSO/Entities/ExamePrestador.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_exameprestador")]
public class ExamePrestador
{
    [Key]
    [Column("cod_prestador")]
    public int CodPrestador { get; set; }

    [Column("nome_prestador")]
    [StringLength(200)]
    public string NomePrestador { get; set; } = string.Empty;

    [Column("cnpj")]
    [StringLength(18)]
    public string? Cnpj { get; set; }

    [Column("endereco")]
    [StringLength(200)]
    public string? Endereco { get; set; }

    [Column("cidade")]
    [StringLength(100)]
    public string? Cidade { get; set; }

    [Column("uf")]
    [StringLength(2)]
    public string? Uf { get; set; }

    [Column("cep")]
    [StringLength(10)]
    public string? Cep { get; set; }

    [Column("telefone")]
    [StringLength(15)]
    public string? Telefone { get; set; }

    [Column("email")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Column("responsavel_tecnico")]
    [StringLength(100)]
    public string? ResponsavelTecnico { get; set; }

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A