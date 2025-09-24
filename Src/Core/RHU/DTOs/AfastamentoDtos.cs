
namespace RhSensoERP.Core.RHU.DTOs
{
    /// <summary>
    /// DTO de item de listagem para Afastamento.
    /// </summary>
    public class AfastamentoListItemDto
    {
            public string NoMatric { get; set; }
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public System.DateTime DtAfast { get; set; }
        public string? CdMotAfas { get; set; }
        public string CdSituacao { get; set; }
    }
}
