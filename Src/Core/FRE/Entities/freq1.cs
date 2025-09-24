using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.FRE.Entities
{
    /// <summary>
    /// Tabela <b>freq1</b> — contém as <b>ocorrências de frequência</b> (faltas, atrasos, HE, abonos).
    /// PK composta: NoMatric, CdEmpresa, CdFilial, DtOcorr, HhIniOcor, TpOcorr.
    /// </summary>
    public class Frequencia : BaseEntity
    {
        public string NoMatric { get; set; } = string.Empty;
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public System.DateTime DtOcorr { get; set; }
        public int TpOcorr { get; set; }
        public string CdMotOc { get; set; } = string.Empty;
        public System.DateTime HhIniOcor { get; set; }
        public System.DateTime HhFimOcor { get; set; }
        public System.DateTime DtFrequen { get; set; }
        public int? QtHorOcor { get; set; }
        public int? QtAbonada { get; set; }
        public int? FlAprovHe { get; set; }
        public int? FlExporta { get; set; }
        public int? QtAdicion { get; set; }
        public string? TxOcorr { get; set; }
        public string? CdCcCusRes { get; set; }
        public string? CdUsuario { get; set; }
        public System.DateTime? DtUltMov { get; set; }
        public System.DateTime? DtTroca { get; set; }
        public string? NoMatTroc { get; set; }
        public string? CdUsAprHe { get; set; }
        public string? NoProcesso { get; set; }
        public int Importado { get; set; }
        public int ContraPesoTroca { get; set; }
        public int FaltouDia { get; set; }
        public int FlBancoHoras { get; set; }
        public int QtMinDescFds { get; set; }
        public string? CdMotOcDefault { get; set; }
        public int? IdBancoHoras { get; set; }
        public int FlHePj { get; set; }
        public System.DateTime? HhIniTroca { get; set; }
        public string? CdUsuarioAceito { get; set; }
        public string? CdUsuarioAutoriza { get; set; }
        public string? CodJustific { get; set; }
        public System.DateTime? DtAceito { get; set; }
        public System.DateTime? DtAutoriza { get; set; }
        public int? FlAceito { get; set; }
        public int? FlAlmoco { get; set; }
        public int? FlAutorizado { get; set; }
        public System.Guid? IdMotivosDeOcorrenciaFrequencia { get; set; }

        /// <summary> Navegação: motivo/ocorrência. </summary>
        public virtual MotivoOcorrenciaFrequencia Motivo { get; set; } = null!;
    }
}
