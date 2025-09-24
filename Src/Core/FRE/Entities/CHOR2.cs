using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.FRE.Entities
{
    /// <summary>
    /// Tabela <b>CHOR2</b> — contém os <b>dias/turnos</b> do horário administrativo (detalhamento).
    /// </summary>
    public class HorarioAdministrativoDia : BaseEntity
    {
        public string CdCargHor { get; set; } = string.Empty; // PK composta com DiaDaSemana
        public int DiaDaSemana { get; set; }                  // 0..6
        public string HhEntrada { get; set; } = string.Empty;
        public string HhSaida { get; set; } = string.Empty;
        public string HhIniInt { get; set; } = string.Empty;
        public string HhFimInt { get; set; } = string.Empty;
        public int FlHabilitado { get; set; }
        public string? CodHors1050 { get; set; }
        public System.Guid? IdHorarioAdministrativo { get; set; }

        public virtual HorarioAdministrativo Horario { get; set; } = null!;
    }
}
