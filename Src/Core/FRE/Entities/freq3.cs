using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.FRE.Entities
{
    /// <summary>
    /// Tabela <b>freq3</b> — contém o <b>resumo de fechamento</b> da frequência por empresa/filial e referência.
    /// </summary>
    public class Frequencia3 : BaseEntity
    {
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public System.DateTime DtFrequen { get; set; }
        public int FlFreq { get; set; }
        public System.Guid IdFilial { get; set; }
    }
}
