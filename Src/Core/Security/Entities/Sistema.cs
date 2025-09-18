using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.Security.Entities;

public class Sistema : BaseEntity
{
    public string CdSistema { get; set; } = string.Empty;
    public string DcSistema { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}