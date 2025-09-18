using RhSensoERP.Core.Abstractions.Entities;

public class GrupoFuncao : BaseEntity
{
    public string CdGrUser { get; set; } = string.Empty;
    public string CdFuncao { get; set; } = string.Empty;
    public string CdAcoes { get; set; } = string.Empty;          // 20 chars: AECIP
    public char CdRestric { get; set; }                          // L/P/C
    public string? CdSistema { get; set; }
    public Guid? IdGrupoDeUsuario { get; set; }

    public virtual Funcao Funcao { get; set; } = null!;
}