namespace RhSensoERP.Identity.Application.DTOs.Permissoes;

public sealed class FuncaoPermissaoDto
{
    public string CdSistema { get; init; } = string.Empty;
    public string CdFuncao  { get; init; } = string.Empty;
    public string? DcFuncao { get; init; }
    public string? DcModulo { get; init; }
    public string Acoes     { get; init; } = string.Empty; // ex: "IAEC"
    public List<BotaoDto> Botoes { get; init; } = new();
}
