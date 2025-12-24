
namespace RhSensoERP.Identity.Core.Entities;

/// <summary>
/// Entidade de associação entre usuários e grupos (tabela usrh1).
/// Cada registro define em qual grupo um usuário está vinculado em um determinado sistema.
/// </summary>
public class UserGroup 
{
    /// <summary>
    /// Código do usuário (cdusuario) - varchar(30) - FK para tuse1.
    /// </summary>
    public string CdUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Código do grupo (cdgruser) - varchar(30) - FK para gurh1.
    /// </summary>
    public string CdGrUser { get; set; } = string.Empty;

    /// <summary>
    /// Código do sistema (cdsistema) - char(10) - FK para tsistema.
    /// </summary>
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Identificador único do grupo (idgrupodeusuario) - uniqueidentifier (opcional).
    /// </summary>
    public Guid? IdGrupoDeUsuario { get; set; }

    // ===============================
    // Relacionamentos
    // ===============================

    /// <summary>
    /// Referência ao usuário.
    /// </summary>
    public virtual Usuario? Usuario { get; set; }

    /// <summary>
    /// Referência ao grupo de usuário.
    /// </summary>
    public virtual GrupoDeUsuario? GrupoDeUsuario { get; set; }

    /// <summary>
    /// Referência ao sistema.
    /// </summary>
    public virtual Tsistema? Sistema { get; set; }
}
