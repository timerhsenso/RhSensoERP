using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.FRE.Entities
{
    /// <summary>
    /// Tabela <b>BATIDAS</b> — contém as <b>marcações de ponto brutas</b> (relógio/coletores).
    /// PK de negócio: CdEmpresa + CdFilial + NoMatric + Data + Hora.
    /// </summary>
    public class Batida : BaseEntity
    {
        public int Id { get; set; } // existe no schema; PK de negócio é composta
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public string NoMatric { get; set; } = string.Empty; // char(8)
        public System.DateTime Data { get; set; }
        public string Hora { get; set; } = string.Empty;     // char(5)
        public string Tipo { get; set; } = string.Empty;     // char(2)
        public string Erro { get; set; } = "0000000000";     // char(10) default
        public int Importado { get; set; } = 0;
        public string? Motivo { get; set; }                  // varchar(200)
        public System.Guid IdGuid { get; set; }
        public System.Guid? IdFuncionario { get; set; }
    }
}
