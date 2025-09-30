namespace RhSensoERP.Application.SEG.DTOs
{
    public sealed class SistemaDto
    {
        public string CdSistema { get; set; } = string.Empty;
        public string DcSistema { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
    }

    public sealed class SistemaUpsertDto
    {
        public string CdSistema { get; set; } = string.Empty;
        public string DcSistema { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
    }
}