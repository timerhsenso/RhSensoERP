
namespace RhSensoERP.Core.RHU.DTOs
{
    /// <summary>
    /// DTO de item de listagem para LancamentoCalculado.
    /// </summary>
    public class LancamentoCalculadoListItemDto
    {
            public string NoMatric { get; set; }
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public string NoProcesso { get; set; }
        public string CdConta { get; set; }
        public double? VlConta { get; set; }
        public double? QtConta { get; set; }
    }
}
