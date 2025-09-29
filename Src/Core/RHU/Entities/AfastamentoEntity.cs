using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.RHU.Entities
{
    /// <summary>
    /// Tabela <b>afas1</b> — contém os <b>afastamentos</b> dos colaboradores (licenças, INSS, etc.).
    /// PK composta: NoMatric, CdEmpresa, CdFilial, DtAfast.
    /// </summary>
    public class Afastamento : BaseEntity
    {
        public string NoMatric { get; set; } = string.Empty;
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public System.DateTime DtAfast { get; set; }

        public System.DateTime? DtBenefic { get; set; }
        public System.DateTime? DtRetorno { get; set; }
        public int? NoSeqHist { get; set; }
        public string CdMotAfas { get; set; } = string.Empty; // char(2)
        public string CdSituacao { get; set; } = string.Empty;// char(2)
        public string? CdSitCadas { get; set; }
        public System.DateTime? DtCat { get; set; }
        public string? NoCat { get; set; }
        public string? CdCid { get; set; }
        public int? CodTpAcidTransito { get; set; }
        public string? NomeMedicoAtestado { get; set; }
        public string? NrOcMedico { get; set; }
        public string? UfOcMedico { get; set; }
        public int? IdEOcMedico { get; set; }
        public int? NoDdAtestado { get; set; }
        public string? InfoMesmoMtv { get; set; }   // char(1)
        public System.Guid? IdFuncionario { get; set; }
        public System.Guid? IdMotivoDeAfastamento { get; set; }
        public System.Guid? IdSituacao { get; set; }
        public System.Guid? IdSituacaoCadastral { get; set; }
        public string? ObsAfast { get; set; }

        public virtual MotivoAfastamento Motivo { get; set; } = null!;
        public virtual Situacao Situacao { get; set; } = null!;
        public virtual Situacao SituacaoCadastral { get; set; } = null!;
    }
}
