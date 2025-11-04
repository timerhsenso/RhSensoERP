using RhSensoERP.Shared.Core.Entities;

namespace RhSensoERP.Identity.Entities;

/// <summary>
/// Entidade que representa o relacionamento usuário-grupo com histórico temporal (tabela usrh1)
/// Mapeia o histórico de grupos que o usuário possui com validade temporal
/// Estrutura baseada no script SQL do banco bd_rhu_copenor
/// </summary>
public class UserGroup : BaseEntity
{
    /// <summary>
    /// Código do usuário - varchar(30) - parte da UK
    /// </summary>
    public string CdUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Código do grupo de usuário - varchar(30) - parte da UK
    /// </summary>
    public string CdGrUser { get; set; } = string.Empty;

    /// <summary>
    /// Data de início da validade - datetime - parte da UK
    /// </summary>
    public DateTime DtIniVal { get; set; }

    /// <summary>
    /// Data de fim da validade - datetime nullable (null = ativo)
    /// </summary>
    public DateTime? DtFimVal { get; set; }

    /// <summary>
    /// Código do sistema - char(10) nullable - parte da UK
    /// </summary>
    public string? CdSistema { get; set; }

    /// <summary>
    /// ID do usuário - uniqueidentifier nullable - FK para tuse1.id
    /// </summary>
    public Guid? IdUsuario { get; set; }

    /// <summary>
    /// ID do grupo de usuário - uniqueidentifier nullable - FK para gurh1.id
    /// </summary>
    public Guid? IdGrupoDeUsuario { get; set; }

    // Relacionamentos
    public virtual User User { get; set; } = null!;
    // Nota: Não temos a entidade GrupoDeUsuario ainda, configuraremos depois
}