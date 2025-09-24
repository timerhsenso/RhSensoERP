using RhSensoERP.Core.Abstractions.Entities;
using System.Collections.Generic;

namespace RhSensoERP.Core.FRE.Entities
{
    /// <summary>
    /// Tabela <b>COMP1</b> — contém a <b>configuração de compensações</b> (escopo).
    /// </summary>
    public class Compensacao : BaseEntity
    {
        public int Id { get; set; }  // PK
        public int? CdEmpresa { get; set; }
        public int? CdFilial { get; set; }
        public string? CdCcusto { get; set; }       // char(5)
        public string Motivo { get; set; } = string.Empty; // varchar(150)
        public string? TpJornada { get; set; }      // char(1)
        public string? CdTurma { get; set; }        // char(2)
        public string? CdCargHor { get; set; }      // char(2)
        public System.DateTime DtUltAlt { get; set; }

        public virtual ICollection<CompensacaoJanela> Janelas { get; set; } = new List<CompensacaoJanela>();
    }
}
