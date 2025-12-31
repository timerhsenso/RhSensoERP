using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Identity.Core.Entities;

[GenerateCrud(
    TableName = "tsistema",
    DisplayName = "Tabela de Sistemas",
    CdSistema = "SEG",
    CdFuncao = "SEG_FM_TSISTEMA",
    IsLegacyTable = true,
    GenerateApiController = true,
    UsePluralRoute = false,
    ApiRoute = "identity/tsistema"
)]
public class Tsistema
{
    [Key]
    [Required]
    [StringLength(10)]
    public string CdsiStema { get; set; } = string.Empty;

    [Required]
    [StringLength(60)]
    public string DcsiStema { get; set; } = string.Empty;

    // ═══════════════════════════════════════════════════════════════
    // NAVEGAÇÃO - Relacionamento 1:N com Funcao
    // ═══════════════════════════════════════════════════════════════
    public virtual ICollection<Funcao> Funcoes { get; set; } = new List<Funcao>();

}
