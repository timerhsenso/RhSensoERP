using RhSensoERP.Core.Abstractions.Entities;
using System.Collections.Generic;

namespace RhSensoERP.Core.FRE.Entities
{
    /// <summary>
    /// Tabela <b>chor1</b> — contém os <b>horários administrativos</b> (grade base).
    /// </summary>
    public class HorarioAdministrativo : BaseEntity
    {
        public string CdCargHor { get; set; } = string.Empty; // char(2)
        public string HhEntrada { get; set; } = string.Empty; // char(5)
        public string HhSaida { get; set; } = string.Empty;   // char(5)
        public string HhIniInt { get; set; } = string.Empty;  // char(5)
        public string HhFimInt { get; set; } = string.Empty;  // char(5)
        public int? MmTolerancia { get; set; }
        public string? FlIntervalo { get; set; }
        public int? MmTolerancia2 { get; set; }
        public string DcCargHor { get; set; } = string.Empty; // varchar(100)
        public string? CodHors1050 { get; set; }

        public virtual ICollection<HorarioAdministrativoDia> Dias { get; set; } = new List<HorarioAdministrativoDia>();
    }
}
