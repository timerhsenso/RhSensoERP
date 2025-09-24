using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.FRE.Entities
{
    /// <summary>
    /// Tabela <b>COMP2</b> — contém as <b>janelas/intervalos</b> de compensação (detalhe da COMP1).
    /// </summary>
    public class CompensacaoJanela : BaseEntity
    {
        public int IdComp { get; set; }             // FK Compensacao
        public System.DateTime Inicio { get; set; } // PK composta
        public System.DateTime Fim { get; set; }
        public int TpOcorr { get; set; }
        public string CdMotOc { get; set; } = string.Empty;   // char(4)

        public virtual Compensacao Compensacao { get; set; } = null!;
    }
}
