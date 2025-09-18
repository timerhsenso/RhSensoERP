using RhSensoERP.Core.Abstractions.Entities;
using RhSensoERP.Core.Security.Entities;

public class UserGroup : BaseEntity
{
    public string CdUsuario { get; set; } = string.Empty;
    public string CdGrUser { get; set; } = string.Empty;
    public DateTime DtIniVal { get; set; }                       // Data início validade
    public DateTime? DtFimVal { get; set; }                      // Data fim validade
    public string? CdSistema { get; set; }
    public Guid? IdUsuario { get; set; }                         // FK tuse1
    public Guid? IdGrupoDeUsuario { get; set; }                  // FK gurh1

    public virtual User User { get; set; } = null!;
}