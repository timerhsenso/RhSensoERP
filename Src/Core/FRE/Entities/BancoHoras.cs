using RhSensoERP.Core.Abstractions.Entities;
using RhSensoERP.Core.RHU.Entities;

namespace RhSensoERP.Core.FRE.Entities
{
    /// <summary>
    /// Tabela <b>BancoHoras</b> — contém as <b>movimentações de banco de horas</b> dos colaboradores.
    /// </summary>
    public class BancoHoras : BaseEntity
    {
        public int Id { get; set; }                // PK (int)
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public string NoMatric { get; set; } = string.Empty;  // char(8)
        public System.DateTime Data { get; set; }
        public int Ordem { get; set; }
        public int Tempo { get; set; }
        public string DebCred { get; set; } = string.Empty;   // char(1)
        public string Tipo { get; set; } = string.Empty;      // char(1)
        public string? Descricao { get; set; }                // varchar(100)
        public string? CdConta { get; set; }                  // char(4) - FK tcon2
        public int SaldoAnterior { get; set; }
        public System.DateTime? DataFreq1 { get; set; }
        public System.DateTime? InicioFreq1 { get; set; }

        /// <summary> Navegação: Verba/conta (tabela tcon2 em RHU) associada, quando houver. </summary>
        public virtual Verba? Conta { get; set; }
    }
}
