using RhSensoERP.Core.Abstractions.Entities;
using System.Collections.Generic;

namespace RhSensoERP.Core.RHU.Entities
{
    /// <summary>
    /// Tabela <b>muni1</b> — contém os <b>municípios</b> (IBGE, UF, nome).
    /// </summary>
    public class Municipio : BaseEntity
    {
        public string CdMunicip { get; set; } = string.Empty; // PK
        public string? SgEstado { get; set; }                 // char(2)
        public string NmMunicip { get; set; } = string.Empty; // varchar(60)
        public int? CodIbge { get; set; }

        public virtual ICollection<CalendarioMunicipal> Calendarios { get; set; } = new List<CalendarioMunicipal>();
    }
}
