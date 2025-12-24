namespace RhSensoERP.Identity.Application.DTOs.Permissoes;

public sealed class BotaoDto
{
    public string NmBotao { get; init; } = string.Empty;
    public string? DcBotao { get; init; }
    public string CdAcao { get; init; } = string.Empty;
}