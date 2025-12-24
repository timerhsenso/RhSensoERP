

namespace RhSensoERP.Identity.Core.Entities;
/// <summary>
/// Entidade que representa uma função/tela do sistema (tabela fucn1)
/// Estrutura baseada no script SQL do banco bd_rhu_copenor
/// </summary>
public class Funcao 
{
    /// <summary>
    /// Código da função - varchar(30) - parte da PK composta
    /// </summary>
    public string CdFuncao { get; set; } = string.Empty;

    /// <summary>
    /// Código do sistema - char(10) - parte da PK composta, FK para tsistema
    /// </summary>
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Descrição da função - varchar(80) - nullable
    /// </summary>
    public string? DcFuncao { get; set; }

    /// <summary>
    /// Descrição do módulo - varchar(100) - nullable
    /// </summary>
    public string? DcModulo { get; set; }

    /// <summary>
    /// Descrição do módulo (campo adicional) - varchar(100) - nullable
    /// </summary>
    public string? DescricaoModulo { get; set; }

    // Relacionamentos
    public virtual Tsistema Sistema { get; set; } = null!;
    public virtual ICollection<BotaoFuncao> Botoes { get; set; } = new List<BotaoFuncao>();
    public virtual ICollection<GrupoFuncao> GrupoFuncoes { get; set; } = new List<GrupoFuncao>();
}