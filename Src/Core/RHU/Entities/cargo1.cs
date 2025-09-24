using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.RHU.Entities
{
    /// <summary>
    /// Tabela <b>cargo1</b> — contém os <b>cargos/funções</b> da organização.
    /// </summary>
    public class Cargo : BaseEntity
    {
        public string CdCargo { get; set; } = string.Empty;     // char(5) - PK
        public string? DcCargo { get; set; }                    // varchar(50)
        public string CdInstruc { get; set; } = string.Empty;   // char(2) - NOT NULL
        public string? CdCbo { get; set; }                      // char(5)
        public string? CdTabela { get; set; }                   // char(3)
        public string? CdNiveIni { get; set; }
        public string? CdNiveFim { get; set; }
        public int FlAtivo { get; set; }
        public string? CdGrProf { get; set; }                   // char(2)
        public string? CdCbo6 { get; set; }                     // char(6)
        public System.DateTime? DtIniVal { get; set; }
        public System.DateTime? DtFimVal { get; set; }
        public int Tenant { get; set; }                         // default 0

        public System.Guid? IdCbo { get; set; }
        public System.Guid? IdGrauDeInstrucao { get; set; }
        public System.Guid? IdTabelaSalarial { get; set; }
    }
}
