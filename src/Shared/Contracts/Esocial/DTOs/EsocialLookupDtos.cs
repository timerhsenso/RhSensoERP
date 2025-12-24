namespace RhSensoERP.Shared.Contracts.Esocial.DTOs;

public sealed record Tabela4LookupDto(string Codigo, string Descricao);

public sealed record Tabela8LookupDto(string Codigo, string Descricao);

public sealed record Tabela10LookupDto(string Codigo, string Descricao);

public sealed record Tabela21LookupDto(string Codigo, string Descricao);

public sealed record LotacaoTributariaLookupDto(
    Guid Id,
    string CodLotacao,
    string Descricao,
    string TpLotacao,
    string TpLotacaoDescricao,
    string Fpas,
    string FpasDescricao
);

public sealed record MotivoOcorrenciaLookupDto(
    Guid Id,
    string CdMotoc,
    int TpOcorr,
    string DcMotoc,
    string TpOcorrDescricao
)
{
    public string DescricaoFormatada => $"{CdMotoc} - {DcMotoc}";
}