
namespace RhSensoERP.Application.RHU.DTOs
{
    /// <summary>
    /// DTO de item de listagem para FeriasProgramacao.
    /// </summary>
    public class FeriasProgramacaoListItemDto
    {
            public string NoMatric { get; set; }
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public System.DateTime DtIniPa { get; set; }
        public int NoSequenc { get; set; }
        public System.DateTime? DtIniPf { get; set; }
        public int? QtDiasFe { get; set; }
        public int? FlConfirm { get; set; }
    }
}
