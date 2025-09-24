using RhSensoERP.Core.Abstractions.Entities;
using System.Collections.Generic;

namespace RhSensoERP.Core.FRE.Entities
{
    /// <summary>
    /// Tabela <b>mfre1</b> — contém os <b>motivos de ocorrência</b> de frequência (faltas, atrasos, etc.).
    /// PK real na origem: TpOcorr + CdMotOc.
    /// </summary>
    public class MotivoOcorrenciaFrequencia : BaseEntity
    {
        public string CdMotOc { get; set; } = string.Empty; // char(4)
        public int TpOcorr { get; set; }                    // int
        public string? DcMotOc { get; set; }               // varchar(40)
        public int? FlMovimen { get; set; }
        public string? CdConta { get; set; }               // char(4)
        public int? FlTpFal { get; set; }
        public int? FlExtra { get; set; }
        public int? FlFlAnj { get; set; }
        public int? FlTroca { get; set; }
        public int? FlRegraHe { get; set; }
        public int FlBancoHoras { get; set; }
        public int? TpOcorrLink { get; set; }
        public string? CdMotOcLink { get; set; }
        public System.Guid? IdMotivoPai { get; set; }
        public System.Guid? IdVerba { get; set; }

        public virtual ICollection<MotivoOcorrenciaFrequenciaEmpresa> EscoposEmpresa { get; set; } = new List<MotivoOcorrenciaFrequenciaEmpresa>();
    }
}
