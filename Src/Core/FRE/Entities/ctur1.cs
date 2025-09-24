using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.FRE.Entities
{
    /// <summary>
    /// Tabela <b>ctur1</b> — contém o <b>calendário/escala</b> de turmas (entrada/saída, repetição).
    /// </summary>
    public class TurmaCalendario : BaseEntity
    {
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public string CdTurma { get; set; } = string.Empty; // char(2)
        public System.DateTime DtCalend { get; set; }
        public string? HhEntrada { get; set; }
        public string? HhSaida { get; set; }
        public string? PontoRepeticao { get; set; }
        public System.Guid? IdTurma { get; set; }
    }
}
