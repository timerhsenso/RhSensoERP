
namespace RhSensoERP.Application.RHU.DTOs
{
    /// <summary>
    /// DTO de item de listagem para CalendarioMunicipal.
    /// </summary>
    public class CalendarioMunicipalListItemDto
    {
            public string CdMunicip { get; set; }
        public System.DateTime DtCalend { get; set; }
        public string CdFeriado { get; set; }
    }
}
