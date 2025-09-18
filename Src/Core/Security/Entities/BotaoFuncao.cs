using RhSensoERP.Core.Abstractions.Entities;

public class BotaoFuncao : BaseEntity
{
    public string CdFuncao { get; set; } = string.Empty;
    public string CdSistema { get; set; } = string.Empty;
    public string NmBotao { get; set; } = string.Empty;
    public string DcBotao { get; set; } = string.Empty;
    public char CdAcao { get; set; }                             // I/A/E/C

    public virtual Funcao Funcao { get; set; } = null!;
}