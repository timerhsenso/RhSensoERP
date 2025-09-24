using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.RHU.Entities
{
    /// <summary>
    /// Tabela <b>tcus1</b> — contém os <b>centros de custo</b> (estrutura organizacional).
    /// </summary>
    public class CentroCusto : BaseEntity
    {
        public string CdCcusto { get; set; } = string.Empty; // PK
        public string DcCcusto { get; set; } = string.Empty;
        public string? SgCcusto { get; set; }
        public string NoCcusto { get; set; } = string.Empty;
        public int? FlAtivo { get; set; }
        public string? DcAreaCracha { get; set; }
        public System.DateTime? DtBloqueio { get; set; }
        public string? CdCcustoPai { get; set; }
        public string? CdCcResp { get; set; }
        public string? FlCcusto { get; set; }
    }
}
