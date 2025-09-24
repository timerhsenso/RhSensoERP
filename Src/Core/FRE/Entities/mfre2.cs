using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.FRE.Entities
{
    /// <summary>
    /// Tabela <b>mfre2</b> — contém o <b>escopo por empresa/filial</b> dos motivos de ocorrência (mfre1).
    /// </summary>
    public class MotivoOcorrenciaFrequenciaEmpresa : BaseEntity
    {
        public string CdMotOc { get; set; } = string.Empty;
        public int TpOcorr { get; set; }
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public System.Guid? IdFilial { get; set; }
        public System.Guid? IdMotivo { get; set; }

        public virtual MotivoOcorrenciaFrequencia Motivo { get; set; } = null!;
    }
}
