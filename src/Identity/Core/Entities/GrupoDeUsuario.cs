
namespace RhSensoERP.Identity.Core.Entities;
/// <summary>
/// Entidade que representa grupos de usuários do sistema (tabela gurh1)
/// Estrutura baseada no script SQL do banco bd_rhu_copenor
/// </summary>
public class GrupoDeUsuario 
{
    /// <summary>
    /// Código do grupo - varchar(30) - parte da PK composta
    /// </summary>
    public string CdGrUser { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do grupo - varchar(60) - nullable
    /// </summary>
    public string? DcGrUser { get; set; }

    /// <summary>
    /// Código do sistema - char(10) - parte da PK composta, FK para tsistema
    /// </summary>
    public string CdSistema { get; set; } = string.Empty;

    // Relacionamentos
    public virtual Tsistema Sistema { get; set; } = null!;
    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    public virtual ICollection<GrupoFuncao> GrupoFuncoes { get; set; } = new List<GrupoFuncao>();
}