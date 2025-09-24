using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.FRE.Entities
{
    /// <summary>
    /// Tabela <b>jornada</b> — contém a <b>jornada mensal</b> por tipo (horas referência).
    /// </summary>
    public class Jornada : BaseEntity
    {
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public string TpJornada { get; set; } = string.Empty; // char(1)
        public int Ano { get; set; }
        public int Mes { get; set; }
        public double? QtHoras { get; set; }
        public System.DateTime? DtRef { get; set; }
    }
}
