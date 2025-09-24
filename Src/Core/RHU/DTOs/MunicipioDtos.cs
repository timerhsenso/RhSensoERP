
namespace RhSensoERP.Core.RHU.DTOs
{
    /// <summary>
    /// DTO de item de listagem para Municipio.
    /// </summary>
    public class MunicipioListItemDto
    {
            public string CdMunicip { get; set; }
        public string NmMunicip { get; set; }
        public string? SgEstado { get; set; }
        public int? CodIbge { get; set; }
    }
}
