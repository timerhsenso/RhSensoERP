using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.Security.Entities;

public class GrupoFuncao : BaseEntity
{
    public string CdGrUser { get; set; } = string.Empty;
    public string CdFuncao { get; set; } = string.Empty;
    public string CdAcoes { get; set; } = string.Empty;
    public char CdRestric { get; set; }
    public string? CdSistema { get; set; }
    public Guid? IdGrupoDeUsuario { get; set; }
}