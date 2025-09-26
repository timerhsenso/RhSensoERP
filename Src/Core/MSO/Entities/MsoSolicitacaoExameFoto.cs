// Src/Core/MSO/Entities/SolicitacaoExameFoto.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_solicitacao_exame_foto")]
public class SolicitacaoExameFoto
{
    [Key]
    [Column("cod_foto")]
    public int CodFoto { get; set; }

    [Column("cod_solicitacao_exame")]
    public int CodSolicitacaoExame { get; set; }

    [Column("nome_arquivo")]
    [StringLength(200)]
    public string NomeArquivo { get; set; } = string.Empty;

    [Column("caminho_arquivo")]
    [StringLength(500)]
    public string CaminhoArquivo { get; set; } = string.Empty;

    [Column("tipo_arquivo")]
    [StringLength(20)]
    public string? TipoArquivo { get; set; }

    [Column("tamanho_arquivo")]
    public long? TamanhoArquivo { get; set; }

    [Column("descricao")]
    [StringLength(200)]
    public string? Descricao { get; set; }

    [Column("data_upload")]
    public DateTime DataUpload { get; set; }

    [Column("cod_usuario")]
    [StringLength(20)]
    public string? CodUsuario { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual SolicitacaoExame? SolicitacaoExame { get; set; }
}
