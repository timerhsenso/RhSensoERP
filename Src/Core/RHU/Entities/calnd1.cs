using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.RHU.Entities
{
    /// <summary>
    /// Tabela <b>calnd1</b> — contém os <b>feriados/ocorrências</b> no calendário municipal.
    /// </summary>
    public class CalendarioMunicipal : BaseEntity
    {
        public string CdMunicip { get; set; } = string.Empty; // char(5)
        public System.DateTime DtCalend { get; set; }
        public string CdFeriado { get; set; } = string.Empty; // char(1)
        public System.Guid? IdMunicipio { get; set; }

        public virtual Municipio Municipio { get; set; } = null!;
    }
}
