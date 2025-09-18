using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.Security.Entities;

public class Funcao : BaseEntity
{
    public string CdFuncao { get; set; } = string.Empty;         // PK composta
    public string CdSistema { get; set; } = string.Empty;        // PK composta
    public string? DcFuncao { get; set; }                        // Descrição
    public string? DcModulo { get; set; }                        // Módulo
    public string? DescricaoModulo { get; set; }                 // Descrição módulo

    public virtual Sistema Sistema { get; set; } = null!;
    public virtual ICollection<BotaoFuncao> Botoes { get; set; } = new List<BotaoFuncao>();
    public virtual ICollection<GrupoFuncao> GrupoFuncoes { get; set; } = new List<GrupoFuncao>();
}