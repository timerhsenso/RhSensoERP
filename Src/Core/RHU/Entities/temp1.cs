using RhSensoERP.Core.Abstractions.Entities;
using System.Collections.Generic;

namespace RhSensoERP.Core.RHU.Entities
{
    /// <summary>
    /// Tabela <b>temp1</b> — contém as <b>empresas</b> (dados cadastrais/legais).
    /// </summary>
    public class Empresa : BaseEntity
    {
        public int CdEmpresa { get; set; }  // PK

        public string? NmEmpresa { get; set; }
        public string? NmFantasia { get; set; }
        public string? NmArqLogo { get; set; }
        public string? NmArqLogoCracha { get; set; }
        public int? FlFapEsocial { get; set; }
        public string? TpInscEmpregador { get; set; }
        public string? NrInscEmpregador { get; set; }
        public string? FlAtivo { get; set; }
        public string? ArquivoLogo { get; set; }
        public byte[]? Logo { get; set; }
        public string? ArquivoLogoCracha { get; set; }
        public byte[]? LogoCracha { get; set; }
        public string? Classtrib { get; set; }
        public string? CnpjEfr { get; set; }
        public System.DateTime? DtDou { get; set; }
        public System.DateTime? DtEmissaoCertificado { get; set; }
        public System.DateTime? DtProtRenovacao { get; set; }
        public System.DateTime? DtVenctoCertificado { get; set; }
        public string? IdEmInLei { get; set; }
        public int? IndAcordoIsenMulta { get; set; }
        public int? IndConstrutora { get; set; }
        public int? IndCooperativa { get; set; }
        public int? IndDesFolha { get; set; }
        public int? IndOpcCp { get; set; }
        public string? IndPorte { get; set; }
        public int? IndOptRegEletronico { get; set; }
        public string? NatJuridica { get; set; }
        public string? NrCertificado { get; set; }
        public string? NrProtRenovacao { get; set; }
        public string? NrRegTt { get; set; }
        public string? PaginaDou { get; set; }

        public virtual ICollection<Filial> Filiais { get; set; } = new List<Filial>();
    }
}
