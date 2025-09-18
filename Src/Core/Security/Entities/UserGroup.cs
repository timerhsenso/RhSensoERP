using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.Security.Entities;

/// <summary>
/// Entidade que representa o relacionamento entre usuário e grupo (tabela usrh1)
/// Mapeia o histórico de grupos que o usuário possui com validade temporal
/// </summary>
public class UserGroup : BaseEntity
{
    /// <summary>
    /// Código do usuário (FK para tuse1.cdusuario)
    /// </summary>
    public string CdUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Código do grupo de usuário
    /// </summary>
    public string CdGrUser { get; set; } = string.Empty;

    /// <summary>
    /// Data de início da validade do grupo para o usuário
    /// </summary>
    public DateTime DtIniVal { get; set; }

    /// <summary>
    /// Data de fim da validade (null = ativo)
    /// </summary>
    public DateTime? DtFimVal { get; set; }

    /// <summary>
    /// Código do sistema
    /// </summary>
    public string? CdSistema { get; set; }

    /// <summary>
    /// ID do usuário (FK para tuse1.id)
    /// </summary>
    public Guid? IdUsuario { get; set; }

    /// <summary>
    /// ID do grupo de usuário (FK para gurh1.id)
    /// </summary>
    public Guid? IdGrupoDeUsuario { get; set; }

    /// <summary>
    /// Relacionamento com a entidade User
    /// </summary>
    public virtual User User { get; set; } = null!;
}