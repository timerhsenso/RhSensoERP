
namespace RhSensoERP.Core.RHU.DTOs
{
    /// <summary>
    /// DTO de item de listagem para Filial.
    /// </summary>
    public class FilialListItemDto
    {
            public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public string? NmFantasia { get; set; }
        public string? DcEstab { get; set; }
    }
}
