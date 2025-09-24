using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.RHU.Entities
{
    /// <summary>
    /// Tabela <b>feria2</b> — contém as <b>programações de férias</b> dos colaboradores.
    /// PK composta: NoMatric, CdEmpresa, CdFilial, DtIniPa, NoSequenc.
    /// </summary>
    public class FeriasProgramacao : BaseEntity
    {
        public string NoMatric { get; set; } = string.Empty;
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public System.DateTime DtIniPa { get; set; }
        public int NoSequenc { get; set; }

        public System.DateTime DtIniPf { get; set; }
        public System.DateTime? DtFimPf { get; set; }
        public int? QtDiasFe { get; set; }
        public int? QtAbono { get; set; }
        public System.DateTime? DtPagto { get; set; }
        public int? FlConfirm { get; set; }
        public int? FlAdia13 { get; set; }
        public string? NoProcesso { get; set; }
        public int Ordem { get; set; }
        public System.Guid? IdPeriodoAquisitivo { get; set; }
        public short Tipo { get; set; }
        public short? SituacaoAntesEnviar { get; set; }
        public System.Guid? IdProcessamento { get; set; }
        public string? FlStatusAprovaAviso { get; set; } // char(2)
        public System.DateTime? DtCienteAviso { get; set; }
        public string? CdUserAprovaAviso { get; set; }
        public System.DateTime? DtAprovaAviso { get; set; }
        public System.DateTime? DtUltMov { get; set; }
        public string? QuemAlterou { get; set; }
    }
}
