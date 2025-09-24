
namespace RhSensoERP.Core.RHU.DTOs
{
    /// <summary>
    /// DTO de item de listagem para FichaFinanceira.
    /// </summary>
    public class FichaFinanceiraListItemDto
    {
            public string NoMatric { get; set; }
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public string CdConta { get; set; }
        public System.DateTime DtConta { get; set; }
        public float? VlConta { get; set; }
        public float? QtConta { get; set; }
    }
}
