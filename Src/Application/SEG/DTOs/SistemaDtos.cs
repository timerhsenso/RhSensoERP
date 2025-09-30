namespace RhSensoERP.Application.SEG.DTOs
{
    /// <summary>
    /// DTO para listagem/detalhe de Sistema.
    /// </summary>
    public sealed class SistemaDto
    {
        public string CdSistema { get; set; } = string.Empty;
        public string DcSistema { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
    }

    /// <summary>
    /// DTO para criação/atualização de Sistema.
    /// </summary>
    public sealed class SistemaUpsertDto
    {
        public string CdSistema { get; set; } = string.Empty;
        public string DcSistema { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
    }
}
